import { copyFileSync, mkdirSync, readFileSync, readdirSync, statSync, writeFileSync } from 'fs';
import { dirname, join } from 'path';

function copyRecursiveSync(src, dest) {
  const exists = statSync(src);
  const isDirectory = exists.isDirectory();
  if (isDirectory) {
    mkdirSync(dest, { recursive: true });
    readdirSync(src).forEach(childItemName => {
      copyRecursiveSync(join(src, childItemName), join(dest, childItemName));
    });
  } else {
    copyFileSync(src, dest);
  }
}

try {
  mkdirSync('dist', { recursive: true });
  copyRecursiveSync('src/command', 'dist/command');
  copyRecursiveSync('src/api', 'dist/api');
  copyRecursiveSync('src/util', 'dist/util');
  copyRecursiveSync('src/database', 'dist/database');
  copyRecursiveSync('TruckersMP-citties-name', 'dist/TruckersMP-citties-name');
  const resourcePath = 'src/resource';
  try {
    copyRecursiveSync(resourcePath, 'dist/resource');
  } catch (err) {
    console.log('Resource directory not found, skipping...');
  }
  writeFileSync('dist/index.mjs', readFileSync('dist/index.js'));
  console.log('Build complete!');
} catch (err) {
  console.error('Build failed:', err);
  process.exit(1);
}