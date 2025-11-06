'use client';

import { useAuth } from './auth/auth-provider';
import { usePathname } from 'next/navigation';

export function LoadingScreen() {
    const { loading } = useAuth();
    const pathname = usePathname();
    const isLoginPage = pathname === '/login';

    // Don't show loading on login page
    if (isLoginPage || !loading) {
        return null;
    }

    return (
        <div className="fixed inset-0 bg-white flex items-center justify-center z-50">
            <div className="flex flex-col items-center gap-4">
                <div className="w-12 h-12 border-4 border-gray-200 border-t-blue-600 rounded-full animate-spin" />
                <p className="text-gray-600">Loading...</p>
            </div>
        </div>
    );
}
