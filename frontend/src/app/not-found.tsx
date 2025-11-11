import Link from 'next/link';

import { Button } from '@/components/ui/button';

export default function NotFound() {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md text-center">
        <div className="bg-white rounded-lg shadow-md p-8">
          <div className="mb-6">
            <h1 className="text-6xl font-bold text-gray-900 mb-2">404</h1>
            <h2 className="text-2xl font-semibold text-gray-800 mb-2">Strona nie znaleziona</h2>
            <p className="text-gray-600">
              Przepraszamy, ale strona której szukasz nie istnieje lub została przeniesiona.
            </p>
          </div>

          <div className="flex flex-col gap-3">
            <Button asChild className="w-full">
              <Link href="/">Wróć do strony głównej</Link>
            </Button>
            <Button asChild variant="outline" className="w-full">
              <Link href="/login">Przejdź do logowania</Link>
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
