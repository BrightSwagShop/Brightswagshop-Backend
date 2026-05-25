function fn() {
  var baseUrl = karate.properties['baseUrl'] ||
    java.lang.System.getenv('FRONTEND_BASE_URL') || 'http://localhost:5173';

  var chromeExe = java.lang.System.getenv('CHROME_EXECUTABLE') ||
    'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe';

  karate.configure('driver', {
    type: 'chrome',
    executable: chromeExe,
    headless: true,
    start: true,
    quit: true
  });

  return {
    baseUrl: baseUrl
  };
}
