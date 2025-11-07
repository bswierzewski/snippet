'use client';

import { useAuth } from '@/components/providers/auth-provider';

export function Navbar() {
  const { signOut } = useAuth();

  return (
    <header className="bg-white border-b border-gray-200">
      <div className="h-16 px-6 flex items-center justify-between">
        {/* Search bar */}
        <div className="flex-1 max-w-2xl">
          {/* Search input will go here */}
        </div>

        {/* Logout button */}
        <button
          onClick={signOut}
          className="px-4 py-2 text-sm text-gray-700 hover:text-gray-900 hover:bg-gray-100 rounded-md transition-colors"
        >
          Wyloguj
        </button>
      </div>

      {/* Filters */}
      <div className="px-6 py-3 border-t border-gray-100 flex gap-4">
        {/* Filters will go here (language, tags, "Clear filters" button) */}
      </div>
    </header>
  );
}
