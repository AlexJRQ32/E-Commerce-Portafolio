/**
 * Formats a numeric value as Costa Rican colon currency (₡1,000.00).
 * If the value is already a formatted string (contains a currency symbol),
 * returns it unchanged (for mock data compatibility).
 *
 * @param {number|string|undefined|null} value - The value to format
 * @returns {string} Formatted currency string
 */
export function formatCurrency(value) {
  // Already formatted (mock data with currency symbols)
  if (typeof value === 'string' && /[₡$€£¥]/.test(value)) {
    return value
  }

  const num = Number(value)
  if (isNaN(num)) {
    return '₡0.00'
  }

  // Format as Costa Rican colon: ₡1,000.00
  return `₡${num.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`
}
