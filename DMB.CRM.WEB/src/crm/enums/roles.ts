export const roles = {
  owner: "owner",
  admin: "admin",
  user: "user",
} as const;

export const writeRoles = [roles.owner, roles.admin];
