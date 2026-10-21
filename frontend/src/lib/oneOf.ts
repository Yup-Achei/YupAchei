export function oneOf<T extends string>(
  value: string,
  allowed: readonly T[],
  label: string,
): T {
  if ((allowed as readonly string[]).includes(value)) {
    return value as T
  }

  throw new Error(`${label} desconhecido: ${value}`)
}
