const indexFiles = require('./index-files-lib.js');

async function Main(){
  await indexFiles.generateFiles();
}

Main();