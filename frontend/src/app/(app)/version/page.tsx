'use client';

import { formatInTimeZone } from 'date-fns-tz';

import { useGetVersion } from '@/lib/api/endpoints/version';

import { Navbar } from '@/components/dashboard/Navbar';
import { Separator } from '@/components/ui/separator';
import { Spinner } from '@/components/ui/spinner';

export default function VersionPage() {
  // Fetch backend version from API using react-query
  const { data: backendVersion, isLoading: loading, error } = useGetVersion();

  // Frontend build info from environment variables (embedded during build)
  const frontendVersion = {
    commitSha: process.env.NEXT_PUBLIC_GIT_COMMIT_SHA || 'unknown',
    buildDate: process.env.NEXT_PUBLIC_BUILD_DATE || 'unknown'
  };

  return (
    <>
      <Navbar />

      <div className="flex-1 overflow-y-auto p-8">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold mb-2">Version Information</h1>
          <p className="text-muted-foreground mb-8">Application version details for both frontend and backend</p>

          <div className="grid gap-8 md:grid-cols-2">
            {/* Frontend Version Card */}
            <div className="rounded-lg border bg-card p-6">
              <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
                <span className="text-2xl">🎨</span>
                Frontend
              </h2>
              <div className="space-y-3">
                <VersionRow
                  label="Commit SHA"
                  value={truncateSha(frontendVersion.commitSha)}
                  title={frontendVersion.commitSha}
                />
                <VersionRow label="Build Date" value={formatDate(frontendVersion.buildDate)} />
                <VersionRow label="Framework" value="Next.js" />
              </div>
            </div>

            {/* Backend Version Card */}
            <div className="rounded-lg border bg-card p-6">
              <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
                <span className="text-2xl">⚙️</span>
                Backend API
              </h2>
              {loading ? (
                <div className="flex items-center justify-center py-8">
                  <Spinner className="h-8 w-8" />
                </div>
              ) : error ? (
                <div className="text-destructive text-sm py-4">
                  Error: {error instanceof Error ? error.message : 'Failed to fetch backend version'}
                </div>
              ) : backendVersion ? (
                <div className="space-y-3">
                  <VersionRow
                    label="Commit SHA"
                    value={truncateSha(backendVersion.commitSha)}
                    title={backendVersion.commitSha}
                  />
                  <VersionRow label="Build Date" value={formatDate(backendVersion.buildDate)} />
                  <VersionRow label="Environment" value={backendVersion.environment} />
                </div>
              ) : null}
            </div>
          </div>

          <Separator className="my-8" />

          {/* Additional Info */}
          <div className="rounded-lg border bg-muted/50 p-6">
            <h3 className="font-semibold mb-2">About Version Information</h3>
            <p className="text-sm text-muted-foreground">
              The commit SHA identifies the exact Git commit used to build each component. This helps verify deployments
              and track which version is currently running. Build dates are in UTC timezone.
            </p>
          </div>
        </div>
      </div>
    </>
  );
}

interface VersionRowProps {
  label: string;
  value: string;
  title?: string;
}

function VersionRow({ label, value, title }: VersionRowProps) {
  return (
    <div className="flex justify-between items-center">
      <span className="text-sm text-muted-foreground">{label}:</span>
      <span className="text-sm font-mono font-medium" title={title}>
        {value}
      </span>
    </div>
  );
}

function formatDate(dateString: string): string {
  if (dateString === 'unknown') return 'unknown';

  try {
    const date = new Date(dateString);
    return formatInTimeZone(date, 'UTC', "MMM d, yyyy, hh:mm a 'UTC'");
  } catch {
    return dateString;
  }
}

function truncateSha(sha: string): string {
  if (sha === 'unknown') return 'unknown';
  return sha.length > 7 ? sha.substring(0, 7) : sha;
}
