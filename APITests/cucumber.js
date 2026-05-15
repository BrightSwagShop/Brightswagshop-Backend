const shared = {
  paths: ['features/**/*.feature'],
  require: ['features/support/**/*.js', 'features/step-definitions/**/*.js'],
  publishQuiet: true
};

const allureFormatter = 'allure-cucumberjs/reporter';

module.exports = {
  default: {
    ...shared,
    format: ['progress', allureFormatter]
  },
  allure: {
    ...shared,
    format: ['progress', allureFormatter]
  },
  testrail: {
    ...shared,
    format: ['progress', allureFormatter, 'json:test-results/cucumber.json', 'junit:./test-results/junit.xml']
  }
};