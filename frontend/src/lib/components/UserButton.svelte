<script lang="ts">
	import { supabase } from '$lib/supabase';

	async function handleSignOut() {
		await supabase.auth.signOut();
	}
</script>

<button
	onclick={handleSignOut}
	class="rounded-lg bg-purple-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2"
>
	Wyloguj
</button>
