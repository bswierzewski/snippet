<script lang="ts">
	import '../app.css';
	import { Auth } from '@supabase/auth-ui-svelte';
	import { ThemeSupa } from '@supabase/auth-ui-shared';
	import { supabase } from '$lib/supabase';
	import { QueryClientProvider } from '@tanstack/svelte-query';
	import { queryClient } from '$lib/query-client';
	import Sidebar from '$lib/components/Sidebar.svelte';
	import { onMount } from 'svelte';
	import type { Session } from '@supabase/supabase-js';

	let { children } = $props();

	// Zarządzanie stanem sesji
	let session = $state<Session | null>(null);
	let loading = $state(true);

	onMount(() => {
		// Pobierz początkową sesję
		supabase.auth.getSession().then(({ data: { session: initialSession } }) => {
			session = initialSession;
			loading = false;
		});

		// Nasłuchuj zmian w sesji (logowanie, wylogowanie)
		const {
			data: { subscription }
		} = supabase.auth.onAuthStateChange((_event, newSession) => {
			session = newSession;
			loading = false;
		});

		return () => subscription.unsubscribe();
	});
</script>

<svelte:head>
	<title>Snippet Manager</title>
</svelte:head>

{#if loading}
	<!-- Ekran ładowania -->
	<div class="flex h-screen items-center justify-center bg-gray-50">
		<div class="text-gray-600">Ładowanie...</div>
	</div>
{:else if !session}
	<!-- Ekran logowania dla niezalogowanych użytkowników -->
	<div class="flex h-screen items-center justify-center bg-gray-50">
		<div class="w-full max-w-md rounded-lg bg-white p-8 shadow-lg">
			<h1 class="mb-6 text-center text-2xl font-bold text-gray-800">Snippet Manager</h1>
			<Auth
				supabaseClient={supabase}
				appearance={{
					theme: ThemeSupa,
					variables: {
						default: {
							colors: {
								brand: '#7c3aed',
								brandAccent: '#6d28d9'
							}
						}
					}
				}}
				providers={['google', 'github']}
				redirectTo={window.location.origin}
				localization={{
					variables: {
						sign_in: {
							email_label: 'Email',
							password_label: 'Hasło',
							email_input_placeholder: 'Twój email',
							password_input_placeholder: 'Twoje hasło',
							button_label: 'Zaloguj się',
							loading_button_label: 'Logowanie...',
							social_provider_text: 'Zaloguj się przez {{provider}}',
							link_text: 'Masz już konto? Zaloguj się',
							magic_link_link_text: 'Wyślij magic link'
						},
						sign_up: {
							email_label: 'Email',
							password_label: 'Hasło',
							email_input_placeholder: 'Twój email',
							password_input_placeholder: 'Twoje hasło',
							button_label: 'Zarejestruj się',
							loading_button_label: 'Rejestracja...',
							social_provider_text: 'Zarejestruj się przez {{provider}}',
							link_text: 'Nie masz konta? Zarejestruj się'
						},
						magic_link: {
							email_input_label: 'Email',
							email_input_placeholder: 'Twój email',
							button_label: 'Wyślij Magic Link',
							loading_button_label: 'Wysyłanie...'
						},
						forgotten_password: {
							link_text: 'Zapomniałeś hasła?',
							button_label: 'Wyślij instrukcje resetowania',
							loading_button_label: 'Wysyłanie...'
						}
					}
				}}
			/>
		</div>
	</div>
{:else}
	<!-- Główna aplikacja dla zalogowanych użytkowników -->
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
{/if}
