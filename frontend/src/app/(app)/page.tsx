'use client';

import { Navbar } from '@/components/dashboard/Navbar';
import { SnippetList } from '@/components/dashboard/SnippetList';

export default function Home() {
  return (
    <>
      {/* Top bar: Search + Filters + Logout */}
      <Navbar />

      {/* Snippet list */}
      <SnippetList />
    </>
  );
}
