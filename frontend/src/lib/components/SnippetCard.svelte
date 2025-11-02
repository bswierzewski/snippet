<script lang="ts">
	import type { SnippetSummaryDto } from '$lib/api/models';

	/**
	 * Komponent karty snippetu
	 * Props:
	 * - snippet: obiekt ze szczegółami snippetu
	 */
	let { snippet }: { snippet: SnippetSummaryDto } = $props();
</script>

<div class="border border-gray-200 rounded-lg p-6 bg-white hover:shadow-md transition-shadow">
	<!-- Nagłówek -->
	<div class="flex items-start justify-between mb-3">
		<div class="flex-1">
			<h3 class="text-lg font-bold text-gray-900 mb-1">{snippet.title}</h3>
			<p class="text-sm text-gray-600">{snippet.description || 'Brak opisu'}</p>
		</div>

		<!-- Badge języka -->
		<span class="px-3 py-1 bg-gray-100 text-gray-700 rounded text-sm font-medium">
			{snippet.language}
		</span>
	</div>

	<!-- Tagi -->
	<div class="flex flex-wrap gap-2 mb-4">
		{#each snippet.tags as tag}
			<span class="px-2 py-1 bg-gray-100 text-gray-600 rounded text-xs">{tag.name}</span>
		{/each}
	</div>

	<!-- Podgląd kodu -->
	<div class="bg-gray-50 rounded p-4 mb-4">
		<pre class="text-sm text-gray-800 overflow-x-auto"><code>{snippet.contentPreview}</code></pre>
	</div>

	<!-- Akcje -->
	<div class="flex items-center justify-between">
		<div class="flex items-center gap-4 text-sm text-gray-500">
			<span>📅 {new Date(snippet.createdAt).toLocaleDateString('pl-PL')}</span>
			{#if snippet.isFavorite}
				<span>⭐ Ulubiony</span>
			{/if}
		</div>

		<div class="flex items-center gap-2">
			<button class="p-2 hover:bg-gray-100 rounded" title="Kopiuj">
				📋
			</button>
			<button class="p-2 hover:bg-gray-100 rounded" title="Edytuj">
				✏️
			</button>
		</div>
	</div>
</div>
