<script lang="ts">
	import '../app.css';
	import favicon from '$lib/assets/favicon.svg';
	import { QueryClientProvider } from '@tanstack/svelte-query';
	import { queryClient } from '$lib/query-client';
	import Sidebar from '$lib/components/Sidebar.svelte';

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<!-- QueryClientProvider udostępnia QueryClient całej aplikacji -->
<QueryClientProvider client={queryClient}>
	<div class="flex h-screen">
		<!-- Lewy panel z kolekcjami -->
		<Sidebar />

		<!-- Główna część aplikacji -->
		<main class="flex-1 overflow-auto">
			{@render children()}
		</main>
	</div>
</QueryClientProvider>
