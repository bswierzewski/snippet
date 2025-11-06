import { createClient } from '@/lib/supabase/middleware';
import { type NextRequest, NextResponse } from 'next/server';

export async function middleware(request: NextRequest) {
  const { supabase, response } = createClient(request);

  // Refresh session if expired - important to call before checking user
  const {
    data: { session }
  } = await supabase.auth.getSession();

  // If user is not logged in and trying to access protected route, redirect to login
  if (!session && request.nextUrl.pathname !== '/login') {
    const url = request.nextUrl.clone();
    url.pathname = '/login';
    return NextResponse.redirect(url);
  }

  // If user is logged in and trying to access login page, redirect to home
  if (session && request.nextUrl.pathname === '/login') {
    const url = request.nextUrl.clone();
    url.pathname = '/';
    return NextResponse.redirect(url);
  }

  return response;
}

export const config = {
  matcher: [
    /*
     * Match all request paths except:
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - public folder
     */
    '/((?!_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp)$).*)'
  ]
};
