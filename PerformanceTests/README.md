# MongoDB API Performance Testing with JMeter

Simple JMeter load test for the SwagShop MongoDB API.

## Prerequisites

### 1. Install JMeter
check:
jmeter -v

### 2. Start Your API
Make sure your API is running:

cd FakeWebShop.Api
dotnet run

## Running the Performance Test

### Option 1: Using PowerShell Script (Easiest)

```powershell
.\run-performance-test.ps1
```

### Option 2: Using JMeter CLI

1. Add JMeter to PATH for this session
$env:Path += ";$HOME\Tools\apache-jmeter-5.6.3\bin"

2. Run the test
jmeter -n -t Simple-LoadTest.jmx -l results/test.jtl -j results/jmeter.log

3. Generate HTML report
jmeter -g results/test.jtl -o results/report
start results/report/index.html

## What the Test Does

**Load Test Configuration:**
- **10 concurrent users**
- **10 loops per user** = 100 total iterations
- **2 requests per iteration** (GET + POST) = **200 total requests**
- **Test duration**: ~5 seconds

**Endpoints Tested:**
1. `GET /api/products` - Retrieves all products
2. `POST /api/products` - Creates a new mug product

## Understanding Results

**Performance Benchmarks:**
- ✅ **Good**: Avg < 10ms, Error rate 0%
- ⚠️ **Acceptable**: Avg 10-50ms, Error rate < 1%
- ❌ **Poor**: Avg > 50ms, Error rate > 1%

## Customizing the Test

To change the load:

1. Open `Simple-LoadTest.jmx` in a text editor or JMeter GUI
2. Find the `<ThreadGroup>` section
3. Modify:
   - `ThreadGroup.num_threads` - Number of concurrent users (default: 10)
   - `LoopController.loops` - Iterations per user (default: 10)
   - `ThreadGroup.ramp_time` - Time to start all users (default: 5 seconds)

## Troubleshooting

**Empty Database?**
Create some test products:

curl -X POST http://localhost:5076/api/products `
  -H "Content-Type: application/json" `
  -d "@sample-mug.json"
