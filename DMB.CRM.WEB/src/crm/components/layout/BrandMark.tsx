const logoSrc = `${process.env.PUBLIC_URL || ""}/dmb-web-solutions-logo.png`;

export default function BrandMark({ compact = false }: { compact?: boolean }) {
  return (
    <span className="brand">
      <img src={logoSrc} alt="CRM" width={48} height={48} />
      <span className="brand-name">
        CRM
        {compact ? null : <span className="brand-sub">Customer Relationship Management</span>}
      </span>
    </span>
  );
}
