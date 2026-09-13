const GIVEN_NAME_CLAIMS = [
  "given_name",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
];

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const part = token.split(".")[1];
    if (!part) return null;
    const padded = part.replace(/-/g, "+").replace(/_/g, "/");
    const json = atob(padded);
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

export function firstNameFromToken(token: string | null): string {
  if (!token) return "";
  const payload = decodeJwtPayload(token);
  if (!payload) return "";
  for (const key of GIVEN_NAME_CLAIMS) {
    const value = payload[key];
    if (typeof value === "string" && value.trim()) {
      return value.trim();
    }
  }
  return "";
}
