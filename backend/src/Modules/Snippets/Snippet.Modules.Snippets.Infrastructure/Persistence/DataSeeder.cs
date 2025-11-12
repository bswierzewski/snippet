using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.Enums;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with mock data for development and testing purposes.
/// </summary>
public class DataSeeder
{
    private readonly SnippetsDbContext _context;
    private static readonly Guid TestUserId = Guid.Parse("37e39a84-1e36-40d7-9e2a-9800e5d18c71");

    public DataSeeder(SnippetsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds the database with mock collections, tags, and snippets.
    /// Creates the database and applies all pending migrations if needed.
    /// </summary>
    public async Task SeedAsync()
    {
        // Ensure database is created and all migrations are applied
        await _context.Database.MigrateAsync();

        // Check if data already exists
        if (await _context.Collections.AnyAsync() || await _context.Snippets.AnyAsync())
        {
            return; // Data already seeded
        }

        // Create collections
        var collections = CreateCollections();
        await _context.Collections.AddRangeAsync(collections);
        await _context.SaveChangesAsync();

        // Create tags
        var tags = CreateTags();
        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync();

        // Create snippets with relationships
        var snippets = CreateSnippets(collections, tags);
        await _context.Snippets.AddRangeAsync(snippets);
        await _context.SaveChangesAsync();
    }

    private static List<Collection> CreateCollections()
    {
        return new List<Collection>
        {
            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Web Development",
                "Frontend and backend web development snippets", "#3B82F6", "🌐", 1),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Database",
                "SQL queries and database management", "#10B981", "🗄️", 2),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "DevOps & CI/CD",
                "Docker, Kubernetes, and deployment scripts", "#8B5CF6", "🚀", 3),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Data Science & ML",
                "Python data analysis and machine learning", "#F59E0B", "🤖", 4),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "API Development",
                "REST API and GraphQL snippets", "#EF4444", "🔌", 5),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Testing",
                "Unit tests, integration tests, and test utilities", "#14B8A6", "✅", 6),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Algorithms",
                "Common algorithms and data structures", "#EC4899", "🧮", 7),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Security",
                "Authentication, encryption, and security best practices", "#DC2626", "🔒", 8),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "UI Components",
                "Reusable React/Vue components", "#06B6D4", "🎨", 9),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Cloud Services",
                "AWS, Azure, GCP code examples", "#6366F1", "☁️", 10),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Git & Version Control",
                "Git commands and workflows", "#F97316", "📝", 11),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Performance",
                "Performance optimization techniques", "#84CC16", "⚡", 12),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Regular Expressions",
                "Regex patterns for common use cases", "#A855F7", "🔍", 13),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Command Line",
                "Bash and PowerShell scripts", "#64748B", "💻", 14),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Documentation",
                "Code documentation templates", "#22C55E", "📚", 15),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Microservices",
                "Microservice patterns and examples", "#F43F5E", "🔗", 16),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Mobile Development",
                "React Native and mobile snippets", "#0EA5E9", "📱", 17),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Utilities",
                "Helper functions and utilities", "#94A3B8", "🛠️", 18),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "AI Prompts",
                "ChatGPT and AI assistant prompts", "#C026D3", "💬", 19),

            new Collection(new CollectionId(Guid.NewGuid()), TestUserId, "Configuration",
                "Config files and environment setup", "#78716C", "⚙️", 20)
        };
    }

    private static List<Tag> CreateTags()
    {
        return new List<Tag>
        {
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "frontend", "#3B82F6"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "backend", "#10B981"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "database", "#F59E0B"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "testing", "#14B8A6"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "docker", "#8B5CF6"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "api", "#EF4444"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "security", "#DC2626"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "performance", "#84CC16"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "typescript", "#06B6D4"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "python", "#F59E0B"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "csharp", "#8B5CF6"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "react", "#06B6D4"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "async", "#EC4899"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "linq", "#A855F7"),
            new Tag(new TagId(Guid.NewGuid()), TestUserId, "git", "#F97316")
        };
    }

    private static List<Domain.Aggregates.Snippet> CreateSnippets(List<Collection> collections, List<Tag> tags)
    {
        var random = new Random(42); // Fixed seed for reproducibility
        var snippets = new List<Domain.Aggregates.Snippet>();

        // 1. React Custom Hook
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "useLocalStorage Hook",
            @"import { useState, useEffect } from 'react';

export function useLocalStorage<T>(key: string, initialValue: T) {
  const [storedValue, setStoredValue] = useState<T>(() => {
    try {
      const item = window.localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch (error) {
      console.error(error);
      return initialValue;
    }
  });

  const setValue = (value: T | ((val: T) => T)) => {
    try {
      const valueToStore = value instanceof Function ? value(storedValue) : value;
      setStoredValue(valueToStore);
      window.localStorage.setItem(key, JSON.stringify(valueToStore));
    } catch (error) {
      console.error(error);
    }
  };

  return [storedValue, setValue] as const;
}",
            ProgrammingLanguage.TypeScript,
            "A custom React hook for managing localStorage with TypeScript support",
            new[] { tags[0], tags[8], tags[11] },
            new[] { collections[0], collections[8] }
        ));

        // 2. SQL Query - Complex JOIN
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "SQL User Activity Report",
            @"SELECT
    u.user_id,
    u.username,
    u.email,
    COUNT(DISTINCT o.order_id) AS total_orders,
    SUM(o.total_amount) AS total_spent,
    AVG(o.total_amount) AS avg_order_value,
    MAX(o.created_at) AS last_order_date
FROM users u
LEFT JOIN orders o ON u.user_id = o.user_id
WHERE u.created_at >= CURRENT_DATE - INTERVAL '1 year'
GROUP BY u.user_id, u.username, u.email
HAVING COUNT(DISTINCT o.order_id) > 0
ORDER BY total_spent DESC
LIMIT 100;",
            ProgrammingLanguage.Sql,
            "Generate a comprehensive user activity report with order statistics",
            new[] { tags[2] },
            new[] { collections[1] }
        ));

        // 3. Docker Compose Configuration
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Full Stack Docker Compose",
            @"version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: myapp
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - '5432:5432'
    healthcheck:
      test: ['CMD-SHELL', 'pg_isready -U admin']
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - '6379:6379'
    volumes:
      - redis_data:/data

  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    environment:
      DATABASE_URL: postgres://admin:${DB_PASSWORD}@postgres:5432/myapp
      REDIS_URL: redis://redis:6379
    ports:
      - '5000:5000'
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_started

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - '3000:3000'
    depends_on:
      - backend

volumes:
  postgres_data:
  redis_data:",
            ProgrammingLanguage.Yaml,
            "Complete Docker Compose setup for full-stack application with PostgreSQL and Redis",
            new[] { tags[4] },
            new[] { collections[2] }
        ));

        // 4. Python Data Analysis
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Pandas Data Cleaning Pipeline",
            @"import pandas as pd
import numpy as np

def clean_dataframe(df: pd.DataFrame) -> pd.DataFrame:
    """"""Clean and preprocess a pandas DataFrame""""""

    # Remove duplicate rows
    df = df.drop_duplicates()

    # Handle missing values
    numeric_columns = df.select_dtypes(include=[np.number]).columns
    df[numeric_columns] = df[numeric_columns].fillna(df[numeric_columns].median())

    categorical_columns = df.select_dtypes(include=['object']).columns
    df[categorical_columns] = df[categorical_columns].fillna(df[categorical_columns].mode().iloc[0])

    # Remove outliers using IQR method
    for col in numeric_columns:
        Q1 = df[col].quantile(0.25)
        Q3 = df[col].quantile(0.75)
        IQR = Q3 - Q1
        lower_bound = Q1 - 1.5 * IQR
        upper_bound = Q3 + 1.5 * IQR
        df = df[(df[col] >= lower_bound) & (df[col] <= upper_bound)]

    # Normalize column names
    df.columns = df.columns.str.lower().str.replace(' ', '_')

    return df

# Usage
df = pd.read_csv('data.csv')
clean_df = clean_dataframe(df)
print(f'Original shape: {df.shape}, Cleaned shape: {clean_df.shape}')",
            ProgrammingLanguage.Python,
            "Comprehensive data cleaning pipeline using pandas",
            new[] { tags[9] },
            new[] { collections[3] }
        ));

        // 5. C# LINQ Extension Method
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "LINQ Batch Extension",
            @"public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source,
        int batchSize)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (batchSize <= 0)
            throw new ArgumentException(""Batch size must be greater than 0"", nameof(batchSize));

        return BatchImpl(source, batchSize);
    }

    private static IEnumerable<IEnumerable<T>> BatchImpl<T>(
        IEnumerable<T> source,
        int batchSize)
    {
        List<T> batch = new(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }
}

// Usage
var numbers = Enumerable.Range(1, 100);
foreach (var batch in numbers.Batch(10))
{
    Console.WriteLine($""Processing batch of {batch.Count()} items"");
}",
            ProgrammingLanguage.CSharp,
            "LINQ extension method to split collections into batches",
            new[] { tags[10], tags[13] },
            new[] { collections[0], collections[17] }
        ));

        // 6. REST API with Error Handling
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Axios Wrapper with Retry",
            @"import axios, { AxiosInstance, AxiosRequestConfig, AxiosError } from 'axios';

class ApiClient {
  private client: AxiosInstance;
  private maxRetries = 3;

  constructor(baseURL: string) {
    this.client = axios.create({
      baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('authToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const config = error.config as AxiosRequestConfig & { _retry?: number };

        if (!config || !config._retry) {
          config._retry = 0;
        }

        if (config._retry < this.maxRetries && this.shouldRetry(error)) {
          config._retry++;
          await this.delay(1000 * config._retry);
          return this.client(config);
        }

        return Promise.reject(error);
      }
    );
  }

  private shouldRetry(error: AxiosError): boolean {
    return !error.response || error.response.status >= 500;
  }

  private delay(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.get<T>(url, config);
    return response.data;
  }

  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.post<T>(url, data, config);
    return response.data;
  }
}

export default ApiClient;",
            ProgrammingLanguage.TypeScript,
            "Axios HTTP client wrapper with automatic retry logic and authentication",
            new[] { tags[5], tags[8] },
            new[] { collections[4] }
        ));

        // 7. Jest Test Suite
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Jest Testing Utilities",
            @"import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

export const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        cacheTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  });

export const renderWithProviders = (
  ui: React.ReactElement,
  { queryClient = createTestQueryClient(), ...renderOptions } = {}
) => {
  const Wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );

  return {
    ...render(ui, { wrapper: Wrapper, ...renderOptions }),
    queryClient,
  };
};

export const waitForLoadingToFinish = () =>
  waitFor(
    () => {
      const loadingElements = screen.queryAllByRole('status');
      expect(loadingElements).toHaveLength(0);
    },
    { timeout: 3000 }
  );

export const fillForm = async (formData: Record<string, string>) => {
  const user = userEvent.setup();

  for (const [name, value] of Object.entries(formData)) {
    const input = screen.getByLabelText(new RegExp(name, 'i'));
    await user.clear(input);
    await user.type(input, value);
  }
};",
            ProgrammingLanguage.TypeScript,
            "Reusable testing utilities for React Testing Library and Jest",
            new[] { tags[3], tags[8], tags[11] },
            new[] { collections[5] }
        ));

        // 8. Binary Search Algorithm
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Generic Binary Search",
            @"public static class SearchAlgorithms
{
    public static int BinarySearch<T>(T[] array, T target) where T : IComparable<T>
    {
        if (array == null || array.Length == 0)
            return -1;

        int left = 0;
        int right = array.Length - 1;

        while (left <= right)
        {
            // Avoid potential overflow
            int mid = left + (right - left) / 2;
            int comparison = array[mid].CompareTo(target);

            if (comparison == 0)
                return mid;

            if (comparison < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1; // Not found
    }

    public static int BinarySearchFirstOccurrence<T>(T[] array, T target)
        where T : IComparable<T>
    {
        int result = -1;
        int left = 0;
        int right = array.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int comparison = array[mid].CompareTo(target);

            if (comparison == 0)
            {
                result = mid;
                right = mid - 1; // Continue searching in left half
            }
            else if (comparison < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return result;
    }
}",
            ProgrammingLanguage.CSharp,
            "Generic binary search implementation with first occurrence variant",
            new[] { tags[10] },
            new[] { collections[6] }
        ));

        // 9. JWT Token Validation
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "JWT Authentication Middleware",
            @"using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers[""Authorization""]
            .FirstOrDefault()?.Split("" "").Last();

        if (token != null)
            await AttachUserToContext(context, token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration[""Jwt:Secret""]!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration[""Jwt:Issuer""],
                ValidateAudience = true,
                ValidAudience = _configuration[""Jwt:Audience""],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            context.Items[""UserId""] = userId;
        }
        catch
        {
            // Token validation failed - do nothing
        }
    }
}",
            ProgrammingLanguage.CSharp,
            "ASP.NET Core middleware for JWT token validation and user authentication",
            new[] { tags[6], tags[1] },
            new[] { collections[7] }
        ));

        // 10. React Table Component
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Sortable Data Table",
            @"import React, { useState, useMemo } from 'react';

interface Column<T> {
  key: keyof T;
  header: string;
  sortable?: boolean;
  render?: (value: any, row: T) => React.ReactNode;
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
}

type SortDirection = 'asc' | 'desc' | null;

export function DataTable<T extends Record<string, any>>({
  data,
  columns
}: DataTableProps<T>) {
  const [sortKey, setSortKey] = useState<keyof T | null>(null);
  const [sortDirection, setSortDirection] = useState<SortDirection>(null);

  const sortedData = useMemo(() => {
    if (!sortKey || !sortDirection) return data;

    return [...data].sort((a, b) => {
      const aVal = a[sortKey];
      const bVal = b[sortKey];

      if (aVal === bVal) return 0;

      const comparison = aVal < bVal ? -1 : 1;
      return sortDirection === 'asc' ? comparison : -comparison;
    });
  }, [data, sortKey, sortDirection]);

  const handleSort = (key: keyof T) => {
    if (sortKey === key) {
      setSortDirection(
        sortDirection === 'asc' ? 'desc' : sortDirection === 'desc' ? null : 'asc'
      );
      if (sortDirection === 'desc') setSortKey(null);
    } else {
      setSortKey(key);
      setSortDirection('asc');
    }
  };

  return (
    <table className=""data-table"">
      <thead>
        <tr>
          {columns.map((column) => (
            <th
              key={String(column.key)}
              onClick={() => column.sortable && handleSort(column.key)}
              className={column.sortable ? 'sortable' : ''}
            >
              {column.header}
              {sortKey === column.key && (
                <span>{sortDirection === 'asc' ? ' ↑' : ' ↓'}</span>
              )}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {sortedData.map((row, idx) => (
          <tr key={idx}>
            {columns.map((column) => (
              <td key={String(column.key)}>
                {column.render
                  ? column.render(row[column.key], row)
                  : row[column.key]}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
}",
            ProgrammingLanguage.TypeScript,
            "Generic sortable data table component for React with TypeScript",
            new[] { tags[0], tags[8], tags[11] },
            new[] { collections[8] }
        ));

        // 11. AWS S3 Upload
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "S3 File Upload Service",
            @"import { S3Client, PutObjectCommand } from '@aws-sdk/client-s3';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';

export class S3Service {
  private s3Client: S3Client;
  private bucketName: string;

  constructor() {
    this.s3Client = new S3Client({
      region: process.env.AWS_REGION!,
      credentials: {
        accessKeyId: process.env.AWS_ACCESS_KEY_ID!,
        secretAccessKey: process.env.AWS_SECRET_ACCESS_KEY!,
      },
    });
    this.bucketName = process.env.S3_BUCKET_NAME!;
  }

  async uploadFile(file: Buffer, key: string, contentType: string): Promise<string> {
    const command = new PutObjectCommand({
      Bucket: this.bucketName,
      Key: key,
      Body: file,
      ContentType: contentType,
    });

    await this.s3Client.send(command);

    return `https://${this.bucketName}.s3.amazonaws.com/${key}`;
  }

  async getPresignedUploadUrl(
    key: string,
    contentType: string,
    expiresIn: number = 3600
  ): Promise<string> {
    const command = new PutObjectCommand({
      Bucket: this.bucketName,
      Key: key,
      ContentType: contentType,
    });

    return await getSignedUrl(this.s3Client, command, { expiresIn });
  }

  generateKey(userId: string, filename: string): string {
    const timestamp = Date.now();
    const sanitized = filename.replace(/[^a-zA-Z0-9.-]/g, '_');
    return `uploads/${userId}/${timestamp}-${sanitized}`;
  }
}",
            ProgrammingLanguage.TypeScript,
            "AWS S3 file upload service with presigned URLs",
            new[] { tags[1], tags[5] },
            new[] { collections[9] }
        ));

        // 12. Git Workflow Commands
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Git Feature Branch Workflow",
            @"# Create and switch to new feature branch
git checkout -b feature/new-feature

# Make changes and stage them
git add .

# Commit with descriptive message
git commit -m ""feat: add new feature description""

# Push to remote and set upstream
git push -u origin feature/new-feature

# Update feature branch with latest main
git checkout main
git pull origin main
git checkout feature/new-feature
git rebase main

# If conflicts occur, resolve them then:
git add .
git rebase --continue

# Squash commits before merging
git rebase -i HEAD~3  # Interactive rebase last 3 commits

# Push rebased branch (force with lease for safety)
git push --force-with-lease

# After PR approval, merge to main
git checkout main
git merge --no-ff feature/new-feature
git push origin main

# Clean up
git branch -d feature/new-feature
git push origin --delete feature/new-feature",
            ProgrammingLanguage.Bash,
            "Complete Git workflow for feature branch development",
            new[] { tags[14] },
            new[] { collections[10] }
        ));

        // 13. Memoization Decorator
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Memoize Decorator",
            @"def memoize(func):
    """"""Cache function results for improved performance""""""
    cache = {}

    def wrapper(*args, **kwargs):
        # Create cache key from arguments
        key = str(args) + str(kwargs)

        if key not in cache:
            cache[key] = func(*args, **kwargs)

        return cache[key]

    wrapper.cache = cache
    wrapper.cache_clear = lambda: cache.clear()
    return wrapper

# Usage example
@memoize
def fibonacci(n):
    if n < 2:
        return n
    return fibonacci(n - 1) + fibonacci(n - 2)

# Calculate Fibonacci numbers
for i in range(10):
    print(f'fib({i}) = {fibonacci(i)}')

# Check cache
print(f'Cache size: {len(fibonacci.cache)}')

# Clear cache
fibonacci.cache_clear()

# Alternative: Use functools.lru_cache
from functools import lru_cache

@lru_cache(maxsize=128)
def factorial(n):
    if n <= 1:
        return 1
    return n * factorial(n - 1)

print(factorial.cache_info())",
            ProgrammingLanguage.Python,
            "Memoization decorator for caching expensive function results",
            new[] { tags[7], tags[9] },
            new[] { collections[11] }
        ));

        // 14. Email Regex Patterns
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Common Regex Patterns",
            @"// Email validation
const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

// URL validation
const urlRegex = /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;

// Phone number (US format)
const phoneRegex = /^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$/;

// Credit card number
const creditCardRegex = /^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13})$/;

// IPv4 address
const ipv4Regex = /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;

// Hex color code
const hexColorRegex = /^#?([a-f0-9]{6}|[a-f0-9]{3})$/i;

// Password strength (min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special)
const strongPasswordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

// Date formats
const dateRegex = /^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$/; // YYYY-MM-DD
const usDateRegex = /^(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01])\/\d{4}$/; // MM/DD/YYYY

// Username (alphanumeric, underscore, hyphen, 3-16 chars)
const usernameRegex = /^[a-zA-Z0-9_-]{3,16}$/;

// Validation function
function validate(pattern: RegExp, value: string): boolean {
  return pattern.test(value);
}

// Usage
console.log(validate(emailRegex, 'test@example.com')); // true
console.log(validate(phoneRegex, '(555) 123-4567')); // true",
            ProgrammingLanguage.JavaScript,
            "Collection of commonly used regular expression patterns for validation",
            null,
            new[] { collections[12], collections[17] }
        ));

        // 15. PowerShell Environment Setup
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "PowerShell Development Setup",
            @"# PowerShell profile setup script
# Location: $PROFILE

# Set execution policy for current user
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Install useful modules
$modules = @(
    'posh-git',
    'PSReadLine',
    'Terminal-Icons'
)

foreach ($module in $modules) {
    if (-not (Get-Module -ListAvailable -Name $module)) {
        Install-Module -Name $module -Scope CurrentUser -Force -AllowClobber
    }
}

# Import modules
Import-Module posh-git
Import-Module PSReadLine
Import-Module Terminal-Icons

# PSReadLine configuration
Set-PSReadLineOption -PredictionSource History
Set-PSReadLineOption -PredictionViewStyle ListView
Set-PSReadLineOption -EditMode Windows
Set-PSReadLineKeyHandler -Key Tab -Function MenuComplete

# Aliases
Set-Alias -Name ll -Value Get-ChildItem
Set-Alias -Name g -Value git
Set-Alias -Name vim -Value nvim

# Functions
function Get-GitStatus { git status }
Set-Alias -Name gs -Value Get-GitStatus

function Open-Explorer { explorer . }
Set-Alias -Name explore -Value Open-Explorer

function Update-Profile {
    . $PROFILE
    Write-Host 'Profile reloaded!' -ForegroundColor Green
}

# Custom prompt
function Prompt {
    $loc = $executionContext.SessionState.Path.CurrentLocation
    $gitBranch = git branch --show-current 2>$null

    Write-Host ""$($loc.Path)"" -NoNewline -ForegroundColor Blue

    if ($gitBranch) {
        Write-Host "" [$gitBranch]"" -NoNewline -ForegroundColor Yellow
    }

    return ""> ""
}

Write-Host ""PowerShell profile loaded!"" -ForegroundColor Green",
            ProgrammingLanguage.PowerShell,
            "Complete PowerShell profile setup with modules, aliases, and custom prompt",
            null,
            new[] { collections[13], collections[19] }
        ));

        // 16. API Documentation Template
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "API Documentation Template",
            @"# API Documentation

## Authentication

All API requests require authentication using a Bearer token in the Authorization header.

```
Authorization: Bearer <your_token>
```

## Base URL

```
https://api.example.com/v1
```

## Endpoints

### Get Users

Returns a list of users.

**Endpoint:** `GET /users`

**Query Parameters:**
- `page` (integer, optional) - Page number (default: 1)
- `limit` (integer, optional) - Items per page (default: 20)
- `search` (string, optional) - Search term

**Response:**
```json
{
  ""data"": [
    {
      ""id"": ""123e4567-e89b-12d3-a456-426614174000"",
      ""email"": ""user@example.com"",
      ""name"": ""John Doe"",
      ""createdAt"": ""2024-01-01T00:00:00Z""
    }
  ],
  ""pagination"": {
    ""page"": 1,
    ""limit"": 20,
    ""total"": 100,
    ""totalPages"": 5
  }
}
```

**Error Responses:**
- `401 Unauthorized` - Invalid or missing token
- `403 Forbidden` - Insufficient permissions
- `500 Internal Server Error` - Server error

### Create User

Creates a new user.

**Endpoint:** `POST /users`

**Request Body:**
```json
{
  ""email"": ""user@example.com"",
  ""name"": ""John Doe"",
  ""password"": ""SecurePass123!""
}
```

**Response:** `201 Created`
```json
{
  ""id"": ""123e4567-e89b-12d3-a456-426614174000"",
  ""email"": ""user@example.com"",
  ""name"": ""John Doe"",
  ""createdAt"": ""2024-01-01T00:00:00Z""
}
```

## Rate Limiting

- 100 requests per minute per IP
- 1000 requests per hour per user

## Error Handling

All errors follow this format:
```json
{
  ""error"": {
    ""code"": ""VALIDATION_ERROR"",
    ""message"": ""Invalid input data"",
    ""details"": [
      {
        ""field"": ""email"",
        ""message"": ""Invalid email format""
      }
    ]
  }
}
```",
            ProgrammingLanguage.Markdown,
            "Comprehensive API documentation template with examples",
            new[] { tags[5] },
            new[] { collections[14] }
        ));

        // 17. Event-Driven Microservice
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "RabbitMQ Event Publisher",
            @"using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public interface IEventPublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T @event) where T : class;
}

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;

    public RabbitMQEventPublisher(
        IConfiguration configuration,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration[""RabbitMQ:Host""],
            Port = int.Parse(configuration[""RabbitMQ:Port""] ?? ""5672""),
            UserName = configuration[""RabbitMQ:Username""],
            Password = configuration[""RabbitMQ:Password""],
            VirtualHost = configuration[""RabbitMQ:VirtualHost""] ?? ""/"",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T @event)
        where T : class
    {
        try
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = ""application/json"";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                ""Published event {EventType} to exchange {Exchange} with routing key {RoutingKey}"",
                typeof(T).Name,
                exchange,
                routingKey);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Failed to publish event {EventType}"", typeof(T).Name);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}",
            ProgrammingLanguage.CSharp,
            "RabbitMQ event publisher for microservices communication",
            new[] { tags[1], tags[12] },
            new[] { collections[15] }
        ));

        // 18. React Native Component
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Animated Button Component",
            @"import React from 'react';
import {
  TouchableOpacity,
  Text,
  StyleSheet,
  Animated,
  ActivityIndicator,
} from 'react-native';

interface AnimatedButtonProps {
  title: string;
  onPress: () => void;
  loading?: boolean;
  disabled?: boolean;
  variant?: 'primary' | 'secondary' | 'outline';
}

export const AnimatedButton: React.FC<AnimatedButtonProps> = ({
  title,
  onPress,
  loading = false,
  disabled = false,
  variant = 'primary',
}) => {
  const scaleAnim = React.useRef(new Animated.Value(1)).current;

  const handlePressIn = () => {
    Animated.spring(scaleAnim, {
      toValue: 0.95,
      useNativeDriver: true,
    }).start();
  };

  const handlePressOut = () => {
    Animated.spring(scaleAnim, {
      toValue: 1,
      friction: 3,
      tension: 40,
      useNativeDriver: true,
    }).start();
  };

  const isDisabled = disabled || loading;

  return (
    <TouchableOpacity
      activeOpacity={0.8}
      disabled={isDisabled}
      onPress={onPress}
      onPressIn={handlePressIn}
      onPressOut={handlePressOut}
    >
      <Animated.View
        style={[
          styles.button,
          styles[variant],
          isDisabled && styles.disabled,
          { transform: [{ scale: scaleAnim }] },
        ]}
      >
        {loading ? (
          <ActivityIndicator color=""#ffffff"" />
        ) : (
          <Text style={[styles.text, styles[`${variant}Text`]]}>{title}</Text>
        )}
      </Animated.View>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  button: {
    paddingVertical: 14,
    paddingHorizontal: 24,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: 50,
  },
  primary: {
    backgroundColor: '#3B82F6',
  },
  secondary: {
    backgroundColor: '#6B7280',
  },
  outline: {
    backgroundColor: 'transparent',
    borderWidth: 2,
    borderColor: '#3B82F6',
  },
  disabled: {
    opacity: 0.5,
  },
  text: {
    fontSize: 16,
    fontWeight: '600',
  },
  primaryText: {
    color: '#FFFFFF',
  },
  secondaryText: {
    color: '#FFFFFF',
  },
  outlineText: {
    color: '#3B82F6',
  },
});",
            ProgrammingLanguage.TypeScript,
            "Animated button component for React Native with loading state",
            new[] { tags[0], tags[8] },
            new[] { collections[16] }
        ));

        // 19. String Formatting Utilities
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "String Utilities Collection",
            @"export const StringUtils = {
  // Convert to camelCase
  toCamelCase(str: string): string {
    return str
      .replace(/(?:^\w|[A-Z]|\b\w)/g, (letter, index) =>
        index === 0 ? letter.toLowerCase() : letter.toUpperCase()
      )
      .replace(/\s+/g, '');
  },

  // Convert to snake_case
  toSnakeCase(str: string): string {
    return str
      .replace(/\W+/g, ' ')
      .split(/ |\B(?=[A-Z])/)
      .map((word) => word.toLowerCase())
      .join('_');
  },

  // Convert to kebab-case
  toKebabCase(str: string): string {
    return str
      .replace(/([a-z])([A-Z])/g, '$1-$2')
      .replace(/[\s_]+/g, '-')
      .toLowerCase();
  },

  // Capitalize first letter
  capitalize(str: string): string {
    return str.charAt(0).toUpperCase() + str.slice(1);
  },

  // Truncate with ellipsis
  truncate(str: string, length: number): string {
    return str.length > length ? str.slice(0, length) + '...' : str;
  },

  // Count words
  wordCount(str: string): number {
    return str.trim().split(/\s+/).length;
  },

  // Remove HTML tags
  stripHtml(str: string): string {
    return str.replace(/<[^>]*>/g, '');
  },

  // Escape HTML
  escapeHtml(str: string): string {
    const map: Record<string, string> = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '""': '&quot;',
      ""'"": '&#039;',
    };
    return str.replace(/[&<>""']/g, (char) => map[char]);
  },

  // Generate slug
  slugify(str: string): string {
    return str
      .toLowerCase()
      .trim()
      .replace(/[^\w\s-]/g, '')
      .replace(/[\s_-]+/g, '-')
      .replace(/^-+|-+$/g, '');
  },

  // Check if palindrome
  isPalindrome(str: string): boolean {
    const cleaned = str.toLowerCase().replace(/[^a-z0-9]/g, '');
    return cleaned === cleaned.split('').reverse().join('');
  },
};

// Usage examples
console.log(StringUtils.toCamelCase('hello world')); // helloWorld
console.log(StringUtils.toSnakeCase('HelloWorld')); // hello_world
console.log(StringUtils.slugify('Hello World! 123')); // hello-world-123",
            ProgrammingLanguage.TypeScript,
            "Collection of useful string manipulation utilities",
            new[] { tags[8] },
            new[] { collections[17] }
        ));

        // 20. ChatGPT Coding Prompt
        snippets.Add(new Domain.Aggregates.Snippet(
            new SnippetId(Guid.NewGuid()), TestUserId,
            "Code Review AI Prompt",
            @"You are an expert code reviewer. Please analyze the following code and provide:

1. **Code Quality Assessment**
   - Overall code structure and organization
   - Adherence to SOLID principles
   - Design patterns usage

2. **Potential Issues**
   - Bugs or logic errors
   - Performance bottlenecks
   - Memory leaks or resource management issues
   - Security vulnerabilities

3. **Best Practices**
   - Code readability and maintainability
   - Naming conventions
   - Comment quality and documentation
   - Error handling

4. **Suggestions for Improvement**
   - Refactoring opportunities
   - Better algorithms or data structures
   - Modern language features that could be used
   - Testing considerations

5. **Security Concerns**
   - Input validation
   - SQL injection or XSS vulnerabilities
   - Authentication/authorization issues
   - Sensitive data handling

Please provide specific examples and code snippets for your suggestions. Rate the code from 1-10 and explain your rating.

---

[PASTE YOUR CODE HERE]

---

After the review, please provide:
- A prioritized list of changes (critical, important, nice-to-have)
- Estimated effort for each suggestion
- Any questions about the code's intent or context",
            ProgrammingLanguage.PlainText,
            "Comprehensive prompt template for AI-assisted code review",
            null,
            new[] { collections[18] }
        ));

        // Mark some as favorites and add usage data
        foreach (var snippet in snippets.Take(5))
        {
            snippet.ToggleFavorite();
            for (int i = 0; i < random.Next(1, 10); i++)
            {
                snippet.RecordUsage();
            }
        }

        return snippets;
    }
}
