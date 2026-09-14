import ManageUsersPanel from "./ManageUsersPanel";
import { roles } from "../../enums/roles";

export default function ManageUsersPage() {
  return (
    <div>
      <h1>Manage users</h1>
      <div className="panel">
        <ManageUsersPanel
          roles={[
            { value: roles.owner, label: "Owner" },
            { value: roles.admin, label: "Admin" },
            { value: roles.user, label: "User" },
          ]}
        />
      </div>
    </div>
  );
}
