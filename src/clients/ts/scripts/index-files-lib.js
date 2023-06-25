const fs = require('fs/promises');
const path = require('path');

async function GetFilesInDirectory(directory, recursive = true) {
  if (!path.isAbsolute(directory)) {
    directory = path.resolve(directory);
  }
  const files = [];
  const subDirectories = [];
  const items = await fs.readdir(directory);
  for (const item of items) {
    const itemPath = `${directory}/${item}`;
    const stat = await fs.stat(itemPath);
    if (stat.isDirectory()) {
      subDirectories.push(itemPath);
    } else {
      files.push(itemPath);
    }
  }

  if (recursive) {
    for (const subDirectory of subDirectories) {
      const subFiles = await GetFilesInDirectory(subDirectory);
      files.push(...subFiles);
    }
  }

  return files;
}

// Delete all "index.ts" files in "src" and subdirectories 
async function deleteIndexFiles() {
  const files = await GetFilesInDirectory('./src');
  const indexFiles = files.filter((file) => file == 'index.ts' || file.endsWith('index.ts'));
  await Promise.all(indexFiles.map((file) => {
    return fs.unlink(`${file}`);
  }));
}

// List all the files in src/models and then re-export them in index.js
async function generateModelsIndex() {
  const files = await GetFilesInDirectory('./src/models', false);
  const models = files
    .map((file) => path.relative(path.resolve("./src/models"), file))
    .filter((file) => file !== 'index.ts')
    .map((file) => file.replace('.ts', ''));
  const index = models.map((model) => `export * from './${model}';`).join('\n');
  await fs.writeFile('src/models/index.ts', index);
}

// List all the files in src and then re-export them in index.ts, also scan for index.ts files in subdirectories
async function generateIndex() {
  await generateModelsIndex();
  const files = await GetFilesInDirectory('./src', false);
  const models = files
    .map((file) => path.relative(path.resolve("./src"), file))
    .filter((file) => file !== 'index.ts')
    .map((file) => file.replace('.ts', ''));
  const index = models.map((model) => `export * from './${model}';`).join('\n');
  await fs.writeFile('src/index.ts', index);
}

exports.deleteFiles = deleteIndexFiles;
exports.generateFiles = generateIndex;

