import { createServerClient } from '@supabase/ssr';
import { cookies } from 'next/headers';

/**
 * Creates Supabase client for Server Components
 *
 * IMPORTANT: This is NOT a singleton (unlike the browser client)
 * Each server request needs a fresh client with request-specific cookies
 *
 * Why? Each HTTP request can be from a different user with different session cookies.
 * Sharing a single client would mix sessions between users = major security issue.
 *
 * Next.js cookies() is request-scoped, so this is safe and necessary.
 */
export async function createClient() {
  const cookieStore = await cookies();

  return createServerClient(process.env.NEXT_PUBLIC_SUPABASE_URL!, process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!, {
    cookies: {
      getAll() {
        return cookieStore.getAll();
      },
      setAll(cookiesToSet) {
        try {
          cookiesToSet.forEach(({ name, value, options }) => cookieStore.set(name, value, options));
        } catch {
          // Silently fail in Server Components where cookies are read-only
        }
      }
    }
  });
}
