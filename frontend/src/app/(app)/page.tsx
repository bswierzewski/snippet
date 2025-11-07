'use client';

import { useAuth } from '@/components/providers/auth-provider';

export default function Home() {
  const { user } = useAuth();

  return (
    <div className="flex items-center justify-center py-20">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">Dashboard</h1>
        {user && <p className="text-gray-600">Zalogowany jako: {user.email}</p>}
      </div>
    </div>
  );
}
