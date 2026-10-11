export const inputCls =
  'w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm ' +
  'focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-100 ' +
  'disabled:bg-slate-50 disabled:text-slate-500'

export const labelCls = 'mb-1 block text-xs font-medium text-slate-500'

export const btnPrimary =
  'rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white ' +
  'hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed'

export const btnSecondary =
  'rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium ' +
  'text-slate-700 hover:bg-slate-50'

export const btnDanger = 'text-sm font-medium text-red-600 hover:text-red-800'

export const btnLink = 'text-sm font-medium text-indigo-600 hover:text-indigo-800'

export function Feedback({
  error,
  success,
}: {
  error?: string | null
  success?: string | null
}) {
  if (error) {
    return (
      <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
        {error}
      </div>
    )
  }
  if (success) {
    return (
      <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
        {success}
      </div>
    )
  }
  return null
}
