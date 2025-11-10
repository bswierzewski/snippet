import { createBrowserClient } from '@supabase/ssr';

/**
 * Creates Supabase client for Client Components (browser)
 *
 * Note: No singleton pattern here - createBrowserClient handles caching internally
 * Safe to call multiple times, it will return the same instance automatically
 */
export function createClient() {
  const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
  const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;

  if (!supabaseUrl || !supabaseAnonKey) {
    throw new Error(
      'Missing Supabase environment variables. Please set NEXT_PUBLIC_SUPABASE_URL and NEXT_PUBLIC_SUPABASE_ANON_KEY'
    );
  }

  return createBrowserClient(supabaseUrl, supabaseAnonKey);
}
