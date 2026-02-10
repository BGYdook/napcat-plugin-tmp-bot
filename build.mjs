import { readFileSync, copyFileSync, mkdirSync, writeFileSync } from 'fs';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

try {
  mkdirSync('dist', { recursive: true });

  copyFileSync(join(__dirname, 'src/index.mjs'), join(__dirname, 'dist/index.mjs'));

  const pkg = JSON.parse(readFileSync(join(__dirname, 'package.json'), 'utf8'));
  const distPkg = {
    name: pkg.name,
    plugin: pkg.plugin,
    version: pkg.version,
    type: pkg.type,
    main: pkg.main,
    description: pkg.description,
    author: pkg.author,
    license: pkg.license,
    keywords: pkg.keywords,
    napcat: pkg.napcat,
    dependencies: {
      'dayjs': '^1.11.13'
    }
  };
  writeFileSync(join(__dirname, 'dist/package.json'), JSON.stringify(distPkg, null, 2));

  console.log('Build complete!');
} catch (err) {
  console.error('Build failed:', err);
  process.exit(1);
}