'use client';

import { useState, useEffect, useRef } from 'react';
import { createClient } from '@/lib/supabase/client';
import { useRouter } from 'next/navigation';
import { Filter, Search, User, Plus } from 'lucide-react';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { CreateSnippetDialog } from './CreateSnippetDialog';
import { ThemeToggle } from './ThemeToggle';
import { FilterDialog } from './FilterDialog';
import { useFilterStore } from '@/lib/store/filterStore';

export function Navbar() {
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isFilterDialogOpen, setIsFilterDialogOpen] = useState(false);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const router = useRouter();
  const supabase = createClient();
  const userMenuRef = useRef<HTMLDivElement>(null);
  const { searchTerm, setSearchTerm, hasActiveMainFilters } = useFilterStore();

  const handleSignOut = async () => {
    await supabase.auth.signOut();
    router.refresh(); // Refresh Server Components - proxy.ts will redirect to /login
  };

  // Close user menu when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    };

    if (isUserMenuOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isUserMenuOpen]);

  return (
    <>
      <header className="bg-background border-b border-border">
        {/* First row: Sidebar toggle + Actions */}
        <div className="h-16 px-6 flex items-center justify-between gap-3">
          {/* Sidebar toggle button */}
          <SidebarTrigger className="p-2 text-muted-foreground hover:text-foreground hover:bg-accent rounded-lg transition-colors shrink-0" />

          {/* Actions */}
          <div className="flex items-center gap-3 shrink-0">
            <button
              onClick={() => setIsCreateDialogOpen(true)}
              className="px-4 py-2 bg-primary text-primary-foreground hover:bg-primary/90 transition-colors flex items-center gap-2 rounded-lg"
              aria-label="Nowy snippet"
            >
              <Plus className="w-5 h-5" />
              <span className="text-sm font-medium">Nowy snippet</span>
            </button>

            <ThemeToggle />

            <div className="relative" ref={userMenuRef}>
              <button
                onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                className="p-2 text-muted-foreground hover:text-foreground hover:bg-accent rounded-lg transition-colors"
                aria-label="User menu"
              >
                <User className="w-5 h-5" />
              </button>

              {/* User dropdown menu */}
              {isUserMenuOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-popover rounded-lg shadow-lg border border-border py-1 z-10">
                  <button
                    onClick={() => {
                      handleSignOut();
                      setIsUserMenuOpen(false);
                    }}
                    className="w-full px-4 py-2 text-left text-sm text-popover-foreground hover:bg-accent"
                  >
                    Wyloguj
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Second row: Search bar + Filter */}
        <div className="px-6 pb-4 flex items-center gap-3">
          {/* Search bar */}
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Szukaj snippetów..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-input bg-background text-foreground rounded-lg focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent"
            />
          </div>

          {/* Filter button */}
          <button
            onClick={() => setIsFilterDialogOpen(true)}
            className="relative p-2 text-muted-foreground hover:text-foreground hover:bg-accent rounded-lg transition-colors shrink-0"
            aria-label="Open filters"
          >
            <Filter className={`w-5 h-5 ${hasActiveMainFilters() ? 'fill-current' : ''}`} />
          </button>
        </div>
      </header>

      <CreateSnippetDialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen} />
      <FilterDialog open={isFilterDialogOpen} onOpenChange={setIsFilterDialogOpen} />
    </>
  );
}
