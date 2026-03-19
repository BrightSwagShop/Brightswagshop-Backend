const token = process.env.QASE_TESTOPS_API_TOKEN || process.env.QASE_API_TOKEN;
const project = process.env.QASE_TESTOPS_PROJECT || process.env.QASE_PROJECT_CODE;

const missing = [];
if (!token) missing.push('QASE_TESTOPS_API_TOKEN (or QASE_API_TOKEN)');
if (!project) missing.push('QASE_TESTOPS_PROJECT (or QASE_PROJECT_CODE)');

if (missing.length > 0) {
  console.error('[Qase] Missing required environment variables for TestOps reporting:');
  for (const envName of missing) {
    console.error(`- ${envName}`);
  }
  process.exit(1);
}

console.log('[Qase] Environment is configured for TestOps reporting.');
