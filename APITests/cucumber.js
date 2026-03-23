const shared = {
  paths: ['features/**/*.feature'],
  require: ['features/step-definitions/**/*.js'],
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
  qase: {
    ...shared,
    tags: '@qase',
    format: ['progress', allureFormatter, 'cucumberjs-qase-reporter']
  }
};
