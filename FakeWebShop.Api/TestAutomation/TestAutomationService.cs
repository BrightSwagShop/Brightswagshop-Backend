using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace FakeWebShop.Api.TestAutomation;

public sealed class TestAutomationService : ITestAutomationService
{
    private const int OutputLimit = 200;

    private readonly ConcurrentDictionary<Guid, TestAutomationRun> _runs = new();
    private readonly ConcurrentDictionary<TestAutomationSuite, Guid> _latestRuns = new();
    private readonly string _backendRoot;
    private readonly string _apiTestsRoot;
    private readonly string _frontendRoot;
    private readonly string _reportRoot;

    public TestAutomationService(IWebHostEnvironment environment)
    {
        _backendRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, ".."));
        _apiTestsRoot = Path.Combine(_backendRoot, "APITests");
        _frontendRoot = Path.GetFullPath(Path.Combine(_backendRoot, "..", "Brightswagshop-Frontend", "frontend"));
        _reportRoot = Path.Combine(environment.ContentRootPath, "wwwroot", "test-automation-runs");
        Directory.CreateDirectory(_reportRoot);
    }

    public Task<TestAutomationRunDto> StartAsync(TestAutomationSuite suite, string? apiBaseUrl = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var runId = Guid.NewGuid();
        var run = new TestAutomationRun(runId, suite, Path.Combine(_reportRoot, runId.ToString("N")));
        _runs[run.Id] = run;
        _latestRuns[suite] = run.Id;

        _ = Task.Run(() => ExecuteRunAsync(run, apiBaseUrl), CancellationToken.None);

        return Task.FromResult(run.ToDto());
    }

    public Task<TestAutomationRunDto?> GetAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_runs.TryGetValue(runId, out var run) ? run.ToDto() : null);
    }

    public Task<IReadOnlyCollection<TestAutomationRunDto>> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var runs = _latestRuns
            .Select(entry => _runs.TryGetValue(entry.Value, out var run) ? run.ToDto() : null)
            .Where(run => run is not null)
            .Cast<TestAutomationRunDto>()
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<TestAutomationRunDto>>(runs);
    }

    private async Task ExecuteRunAsync(TestAutomationRun run, string? apiBaseUrl)
    {
        try
        {
            run.MarkRunning();

            var execution = run.Suite switch
            {
                TestAutomationSuite.Api => new TestExecutionPlan(
                    ResolveExecutable("npx"),
                    new[] { "cucumber-js", "--profile", "default" },
                    _apiTestsRoot,
                    GetProcessEnvironmentForSuite(run.Suite, apiBaseUrl)),
                TestAutomationSuite.Frontend => new TestExecutionPlan(
                    ResolveExecutable("npm"),
                    new[] { "run", "webtests" },
                    _frontendRoot,
                    new Dictionary<string, string>()),
                TestAutomationSuite.E2e => new TestExecutionPlan(
                    ResolveExecutable("npm"),
                    new[] { "run", "webtests" },
                    _frontendRoot,
                    GetProcessEnvironmentForSuite(run.Suite)),
                _ => throw new NotSupportedException($"Unknown test suite: {run.Suite}")
            };

            var exitCode = await RunProcessAsync(execution, run);
            run.MarkCompleted(exitCode, exitCode == 0 ? null : $"Test process exited with code {exitCode}.");

            await PublishReportAsync(run);
        }
        catch (Exception exception)
        {
            run.MarkFailed(exception.Message);
            await PublishFallbackReportAsync(run);
        }
    }

    private async Task<int> RunProcessAsync(TestExecutionPlan execution, TestAutomationRun run)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = ResolveCommandPath(execution.FileName),
            WorkingDirectory = execution.WorkingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var argument in execution.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        foreach (var environmentVariable in execution.EnvironmentVariables)
        {
            startInfo.Environment[environmentVariable.Key] = environmentVariable.Value;
        }

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        process.OutputDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.Data))
            {
                run.AppendOutput(eventArgs.Data);
            }
        };

        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.Data))
            {
                run.AppendOutput(eventArgs.Data);
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException($"Failed to start {execution.FileName}.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return process.ExitCode;
    }

    private async Task PublishReportAsync(TestAutomationRun run)
    {
        Directory.CreateDirectory(run.ReportRoot);

        if (run.Suite is TestAutomationSuite.Frontend or TestAutomationSuite.E2e)
        {
            var playwrightReportRoot = Path.Combine(_frontendRoot, "playwright-report");

            if (Directory.Exists(playwrightReportRoot))
            {
                CopyDirectory(playwrightReportRoot, run.ReportRoot);
                return;
            }
        }

        await PublishFallbackReportAsync(run);
    }

    private async Task PublishFallbackReportAsync(TestAutomationRun run)
    {
        Directory.CreateDirectory(run.ReportRoot);

        var reportFile = Path.Combine(run.ReportRoot, "index.html");
        var html = BuildReportHtml(run);
        await File.WriteAllTextAsync(reportFile, html, Encoding.UTF8);
    }

    private static string BuildReportHtml(TestAutomationRun run)
    {
        var statusColor = run.Status switch
        {
            TestAutomationRunStatus.Succeeded => "#047857",
            TestAutomationRunStatus.Failed => "#b91c1c",
            TestAutomationRunStatus.Running => "#b45309",
            _ => "#0f172a"
        };

        var output = string.Join(Environment.NewLine, run.GetOutputTail().Select(WebUtility.HtmlEncode));
        var errorSection = string.IsNullOrWhiteSpace(run.ErrorMessage)
            ? string.Empty
            : $"<section><h2>Failure</h2><pre>{WebUtility.HtmlEncode(run.ErrorMessage)}</pre></section>";

        var startedText = run.StartedAt.ToString("O");
        var completedText = run.CompletedAt?.ToString("O") ?? "In progress";
        var exitCodeText = run.ExitCode?.ToString() ?? "N/A";
        var html = new StringBuilder();

        html.AppendLine("<!doctype html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("  <meta charset=\"utf-8\" />");
        html.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        html.AppendLine("  <title>Test run " + run.Id.ToString("N") + "</title>");
        html.AppendLine("  <style>");
        html.AppendLine("    body { font-family: Arial, sans-serif; margin: 0; padding: 24px; background: #f8fafc; color: #0f172a; }");
        html.AppendLine("    .status { display: inline-block; padding: 8px 14px; border-radius: 999px; color: white; background: " + statusColor + "; font-weight: 700; }");
        html.AppendLine("    section { background: white; border: 1px solid #e2e8f0; border-radius: 16px; padding: 18px; margin-top: 18px; }");
        html.AppendLine("    pre { white-space: pre-wrap; word-break: break-word; background: #0f172a; color: #e2e8f0; padding: 16px; border-radius: 12px; overflow: auto; }");
        html.AppendLine("    .meta { display: grid; gap: 8px; margin-top: 12px; }");
        html.AppendLine("    .meta div { display: flex; gap: 12px; flex-wrap: wrap; }");
        html.AppendLine("    .label { font-weight: 700; min-width: 120px; }");
        html.AppendLine("  </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("  <h1>Test automation report</h1>");
        html.AppendLine("  <div class=\"status\">" + run.Status + "</div>");
        html.AppendLine("  <section>");
        html.AppendLine("    <div class=\"meta\">");
        html.AppendLine("      <div><span class=\"label\">Suite</span><span>" + run.Suite + "</span></div>");
        html.AppendLine("      <div><span class=\"label\">Started</span><span>" + startedText + "</span></div>");
        html.AppendLine("      <div><span class=\"label\">Completed</span><span>" + completedText + "</span></div>");
        html.AppendLine("      <div><span class=\"label\">Exit code</span><span>" + exitCodeText + "</span></div>");
        html.AppendLine("    </div>");
        html.AppendLine("  </section>");

        if (!string.IsNullOrWhiteSpace(errorSection))
        {
            html.AppendLine(errorSection);
        }

        html.AppendLine("  <section>");
        html.AppendLine("    <h2>Latest output</h2>");
        html.AppendLine("    <pre>" + output + "</pre>");
        html.AppendLine("  </section>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private static string ResolveExecutable(string executable)
    {
        if (OperatingSystem.IsWindows())
        {
            return executable switch
            {
                "dotnet" => "dotnet.exe",
                "npm" => "npm.cmd",
                "npx" => "npx.cmd",
                _ => executable
            };
        }

        return executable;
    }

    private static string ResolveCommandPath(string commandName)
    {
        if (Path.IsPathRooted(commandName) && File.Exists(commandName))
        {
            return commandName;
        }

        var pathEnvironment = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var searchPaths = pathEnvironment.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string[] extensions = OperatingSystem.IsWindows()
            ? (Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD")
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : [string.Empty];

        var candidateNames = new List<string> { commandName };

        if (!Path.HasExtension(commandName) && OperatingSystem.IsWindows())
        {
            candidateNames.AddRange(extensions.Select(extension => commandName + extension));
        }

        foreach (var searchPath in searchPaths)
        {
            foreach (var candidateName in candidateNames)
            {
                var candidatePath = Path.Combine(searchPath, candidateName);
                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }
            }
        }

        return commandName;
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);

        foreach (var directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var targetDirectory = directory.Replace(sourceDirectory, destinationDirectory);
            Directory.CreateDirectory(targetDirectory);
        }

        foreach (var file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var targetFile = file.Replace(sourceDirectory, destinationDirectory);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
            File.Copy(file, targetFile, true);
        }
    }

    private sealed record TestExecutionPlan(
        string FileName,
        IReadOnlyCollection<string> Arguments,
        string WorkingDirectory,
        IReadOnlyDictionary<string, string> EnvironmentVariables);

    private IReadOnlyDictionary<string, string> GetProcessEnvironmentForSuite(TestAutomationSuite suite, string? apiBaseUrl = null)
    {
        if (suite == TestAutomationSuite.Api)
        {
            var configuredApiBaseUrl = !string.IsNullOrWhiteSpace(apiBaseUrl)
                ? apiBaseUrl.Trim()
                : Environment.GetEnvironmentVariable("API_BASE_URL")?.Trim();

            var apiVariables = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(configuredApiBaseUrl))
            {
                apiVariables["API_BASE_URL"] = configuredApiBaseUrl;
            }

            return apiVariables;
        }

        if (suite != TestAutomationSuite.E2e)
        {
            return new Dictionary<string, string>();
        }

        var configuredBaseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")?.Trim();
        var fallbackBaseUrl = Environment.GetEnvironmentVariable("Frontend__ProductionBaseUrl")?.Trim();
        var resolvedBaseUrl = !string.IsNullOrWhiteSpace(configuredBaseUrl)
            ? configuredBaseUrl
            : fallbackBaseUrl ?? string.Empty;

        var variables = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(resolvedBaseUrl))
        {
            variables["PLAYWRIGHT_BASE_URL"] = resolvedBaseUrl;
        }

        return variables;
    }

    private sealed class TestAutomationRun
    {
        private readonly object _sync = new();
        private readonly List<string> _outputTail = new();

        public TestAutomationRun(Guid id, TestAutomationSuite suite, string reportRoot)
        {
            Id = id;
            Suite = suite;
            ReportRoot = reportRoot;
            StartedAt = DateTimeOffset.UtcNow;
            ReportPath = $"/test-automation-runs/{id:N}/index.html";
            Status = TestAutomationRunStatus.Queued;
        }

        public Guid Id { get; }

        public TestAutomationSuite Suite { get; }

        public string ReportRoot { get; }

        public string ReportPath { get; }

        public TestAutomationRunStatus Status { get; private set; }

        public DateTimeOffset StartedAt { get; private set; }

        public DateTimeOffset? CompletedAt { get; private set; }

        public int? ExitCode { get; private set; }

        public string? ErrorMessage { get; private set; }

        public TestAutomationRunDto ToDto()
        {
            return new TestAutomationRunDto(
                Id,
                Suite,
                Status,
                StartedAt,
                CompletedAt,
                ExitCode,
                ReportPath,
                GetOutputTail(),
                ErrorMessage);
        }

        public IReadOnlyCollection<string> GetOutputTail()
        {
            lock (_sync)
            {
                return _outputTail.ToArray();
            }
        }

        public void AppendOutput(string line)
        {
            lock (_sync)
            {
                _outputTail.Add(line);

                if (_outputTail.Count > OutputLimit)
                {
                    _outputTail.RemoveRange(0, _outputTail.Count - OutputLimit);
                }
            }
        }

        public void MarkRunning()
        {
            Status = TestAutomationRunStatus.Running;
        }

        public void MarkCompleted(int exitCode, string? errorMessage)
        {
            Status = exitCode == 0 ? TestAutomationRunStatus.Succeeded : TestAutomationRunStatus.Failed;
            ExitCode = exitCode;
            ErrorMessage = errorMessage;
            CompletedAt = DateTimeOffset.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            Status = TestAutomationRunStatus.Failed;
            ErrorMessage = errorMessage;
            CompletedAt = DateTimeOffset.UtcNow;
        }
    }
}