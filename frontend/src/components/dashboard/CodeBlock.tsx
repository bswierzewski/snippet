'use client';

import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { oneDark, oneLight } from 'react-syntax-highlighter/dist/esm/styles/prism';
import { useTheme } from 'next-themes';
import { useEffect, useState } from 'react';

interface CodeBlockProps {
  code: string;
  language: string;
}

// Map language names to syntax highlighter language keys
const languageMap: Record<string, string> = {
  'C#': 'csharp',
  'C++': 'cpp',
  'JavaScript': 'javascript',
  'TypeScript': 'typescript',
  'Python': 'python',
  'Java': 'java',
  'Ruby': 'ruby',
  'Go': 'go',
  'Rust': 'rust',
  'PHP': 'php',
  'Swift': 'swift',
  'Kotlin': 'kotlin',
  'Scala': 'scala',
  'R': 'r',
  'Perl': 'perl',
  'Shell': 'bash',
  'Bash': 'bash',
  'SQL': 'sql',
  'HTML': 'markup',
  'CSS': 'css',
  'SCSS': 'scss',
  'JSON': 'json',
  'YAML': 'yaml',
  'XML': 'xml',
  'Markdown': 'markdown',
  'Dart': 'dart',
  'Lua': 'lua',
  'Haskell': 'haskell',
  'Elixir': 'elixir',
  'Clojure': 'clojure',
  'F#': 'fsharp',
  'Objective-C': 'objectivec',
  'PowerShell': 'powershell',
  'VB.NET': 'vbnet',
};

export function CodeBlock({ code, language }: CodeBlockProps) {
  const { theme, systemTheme } = useTheme();
  const [mounted, setMounted] = useState(false);
  const highlighterLanguage = languageMap[language] || 'text';

  // useEffect only runs on the client, so now we can safely show the UI
  useEffect(() => {
    setMounted(true);
  }, []);

  // Determine the actual theme (accounting for system theme)
  const currentTheme = theme === 'system' ? systemTheme : theme;
  const isDark = currentTheme === 'dark';

  // Show a placeholder during SSR to avoid hydration mismatch
  if (!mounted) {
    return (
      <div className="bg-muted rounded-lg p-3">
        <pre className="text-xs font-mono text-foreground">{code}</pre>
      </div>
    );
  }

  return (
    <SyntaxHighlighter
      language={highlighterLanguage}
      style={isDark ? oneDark : oneLight}
      customStyle={{
        margin: 0,
        borderRadius: '0.5rem',
        fontSize: '0.75rem',
        padding: '0.75rem',
      }}
      showLineNumbers={false}
      wrapLines={true}
      wrapLongLines={true}
    >
      {code}
    </SyntaxHighlighter>
  );
}
