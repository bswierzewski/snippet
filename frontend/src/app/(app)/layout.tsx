'use client';

import { useIsMobile } from '@/hooks/use-mobile';

import { AppSidebar } from '@/components/dashboard/AppSidebar';
import { SidebarInset, SidebarProvider } from '@/components/ui/sidebar';

export default function AppLayout({ children }: { children: React.ReactNode }) {
  const isMobile = useIsMobile();

  return (
    <SidebarProvider defaultOpen={!isMobile}>
      <AppSidebar />
      <SidebarInset className="flex flex-col">{children}</SidebarInset>
    </SidebarProvider>
  );
}
