const { spawn } = require('child_process');

const args = process.argv.slice(2);

const migrationName = args[0];

const dotnetArgs = [
    'ef', 'migrations', 'add', migrationName,
    '--project', './src/Core',
    '--startup-project', './src/Web'
];

const proc = spawn('dotnet', dotnetArgs, { stdio: 'inherit' });

proc.on('exit', code => process.exit(code));
