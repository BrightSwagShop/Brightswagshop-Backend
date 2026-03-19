const shared = {
  paths: ['features/**/*.feature'],
  require: ['features/step-definitions/**/*.js'],
  publishQuiet: true
};

module.exports = {
  default: {
    ...shared,
    format: ['progress']
  },
  qase: {
    ...shared,
    format: ['progress', 'cucumberjs-qase-reporter']
  }
};
