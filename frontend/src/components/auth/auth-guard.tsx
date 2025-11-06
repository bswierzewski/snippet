'use client';

import { useAuth } from './auth-provider';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect } from 'react';

/**
 * Protects all routes except /login
 * Redirects unauthenticated users to login page
 */
export function AuthGuard({ children }: { children: React.ReactNode }) {
    const { user, loading } = useAuth();
    const pathname = usePathname();
    const router = useRouter();
    const isLoginPage = pathname === '/login';

    // Redirect to login if user is not authenticated
    useEffect(() => {
        if (!loading && !user && !isLoginPage) {
            router.push('/login');
        }
    }, [user, loading, isLoginPage, router]);

    // Allow access to login page without authentication
    if (isLoginPage) {
        return <>{children}</>;
    }

    // Hide content while checking auth or redirecting to prevent flash of unauthorized content
    if (loading || !user) {
        return null;
    }

    return <>{children}</>;
}
