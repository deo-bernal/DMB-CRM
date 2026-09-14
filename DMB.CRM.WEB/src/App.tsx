import { Navigate, Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "./router/ProtectedRoute";
import Shell from "./crm/components/layout/Shell";
import SiteFooter from "./crm/components/layout/SiteFooter";
import LoadingModal from "./crm/components/layout/LoadingModal";
import Login from "./crm/components/auth/Login";
import AuthCallback from "./crm/components/auth/AuthCallback";
import AuthComplete from "./crm/components/auth/AuthComplete";
import Register from "./crm/components/auth/Register";
import ForgotPassword from "./crm/components/auth/ForgotPassword";
import ResetPassword from "./crm/components/auth/ResetPassword";
import Activate from "./crm/components/auth/Activate";
import Dashboard from "./crm/components/dashboard/Dashboard";
import ContactList from "./crm/components/contacts/List/List";
import ContactCreate from "./crm/components/contacts/Create/Create";
import ContactView from "./crm/components/contacts/View/View";
import CompanyList from "./crm/components/companies/List/List";
import TagList from "./crm/components/tags/List/List";
import OpportunityList from "./crm/components/opportunities/List/List";
import ManageUsersPage from "./crm/components/admin/ManageUsersPage";
import AccountPage from "./crm/components/account/AccountPage";
import { writeRoles } from "./crm/enums/roles";

export default function App() {
  return (
    <div className="app-frame">
      <div className="app-body">
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/auth/callback" element={<AuthCallback />} />
          <Route path="/auth/complete" element={<AuthComplete />} />
          <Route path="/register" element={<Register />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/activate" element={<Activate />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Shell />
              </ProtectedRoute>
            }
          >
            <Route index element={<Dashboard />} />
            <Route path="contacts" element={<ContactList />} />
            <Route path="contacts/create" element={<ProtectedRoute allowedRoles={[...writeRoles]}><ContactCreate /></ProtectedRoute>} />
            <Route path="contacts/create/:id" element={<ProtectedRoute allowedRoles={[...writeRoles]}><ContactCreate /></ProtectedRoute>} />
            <Route path="contacts/view/:id" element={<ContactView />} />
            <Route path="companies" element={<CompanyList />} />
            <Route path="tags" element={<TagList />} />
            <Route path="opportunities" element={<OpportunityList />} />
            <Route path="users" element={<ProtectedRoute allowedRoles={[...writeRoles]}><ManageUsersPage /></ProtectedRoute>} />
            <Route path="account" element={<AccountPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
      <SiteFooter />
      <LoadingModal />
    </div>
  );
}
