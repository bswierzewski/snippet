'use client';

export function Sidebar() {
  return (
    <aside className="w-[280px] bg-gray-50 border-r border-gray-200 flex flex-col">
      {/* Logo/Header */}
      <div className="h-16 px-4 flex items-center border-b border-gray-200">
        <h1 className="text-xl font-semibold text-gray-900">SnippetVault</h1>
      </div>

      {/* Sidebar content */}
      <div className="flex-1 overflow-y-auto p-4">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-sm font-semibold text-gray-600">KOLEKCJE</h2>
          {/* Add new collection button will go here */}
        </div>

        {/* Collections section */}
        <div className="mb-4">
          {/* Collections list will go here */}
        </div>
      </div>
    </aside>
  );
}
