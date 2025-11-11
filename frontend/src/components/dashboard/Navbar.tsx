'use client';

import { useState } from 'react';
import { createClient } from '@/lib/supabase/client';
import { useRouter } from 'next/navigation';
import { CreateSnippetDialog } from './CreateSnippetDialog';

export function Navbar() {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const router = useRouter();
  const supabase = createClient();

  const handleSignOut = async () => {
    await supabase.auth.signOut();
    router.refresh(); // Refresh Server Components - proxy.ts will redirect to /login
  };

  return (
    <>
      <header className="bg-white border-b border-gray-200">
        <div className="h-16 px-6 flex items-center justify-between">
          {/* Search bar */}
          <div className="flex-1 max-w-2xl">
            {/* Search input will go here */}
          </div>

          {/* Actions */}
          <div className="flex items-center gap-3">
            <button
              onClick={() => setIsDialogOpen(true)}
              className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 flex items-center gap-2"
            >
              <span className="text-xl">+</span>
              Nowy snippet
            </button>

            <button
              onClick={handleSignOut}
              className="px-4 py-2 text-sm text-gray-700 hover:text-gray-900 hover:bg-gray-100 rounded-md transition-colors"
            >
              Wyloguj
            </button>
          </div>
        </div>

        {/* Filters */}
        <div className="px-6 py-3 border-t border-gray-100 flex gap-4">
          {/* Filters will go here (language, tags, "Clear filters" button) */}
        </div>
      </header>
      <CreateSnippetDialog open={isDialogOpen} onOpenChange={setIsDialogOpen} />
    </>
  );
}
