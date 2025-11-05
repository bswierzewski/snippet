import { createClient } from '@supabase/supabase-js';
import { PUBLIC_SUPABASE_URL, PUBLIC_SUPABASE_ANON_KEY } from '$env/static/public';

export const supabase = createClient(PUBLIC_SUPABASE_URL, PUBLIC_SUPABASE_ANON_KEY);

/**
 * Get current auth session
 */
export async function getSession() {
	const { data, error } = await supabase.auth.getSession();
	if (error) {
		console.error('Error getting session:', error);
		return null;
	}
	return data.session;
}

/**
 * Get current access token for API calls
 */
export async function getAccessToken() {
	const session = await getSession();
	return session?.access_token ?? null;
}
