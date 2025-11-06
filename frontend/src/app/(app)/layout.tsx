'use client';

import { useAuth } from '@/components/auth/auth-provider';

export default function AppLayout({ children }: { children: React.ReactNode }) {
  const { signOut } = useAuth();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header - możesz rozbudować w przyszłości */}
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
          <h1 className="text-xl font-semibold text-gray-900">SnippetVault</h1>
          <button
            onClick={signOut}
            className="px-4 py-2 text-sm text-gray-700 hover:text-gray-900 hover:bg-gray-100 rounded-md transition-colors"
          >
            Wyloguj
          </button>
        </div>
      </header>

      {/* Main content */}
      <main>{children}</main>
    </div>
  );
}
