'use client';

import { Sidebar } from '@/components/dashboard/Sidebar';

export default function AppLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="h-screen flex">
      {/* Left sidebar - full height */}
      <Sidebar />

      {/* Main content area (navbar + snippets) */}
      <div className="flex-1 flex flex-col">
        {children}
      </div>
    </div>
  );
}
