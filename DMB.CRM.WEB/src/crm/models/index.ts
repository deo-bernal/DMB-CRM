export type LocationMembership = {
  locationId: string;
  name: string;
  role: string;
};

export type LoginResponse = {
  token: string;
  locations: LocationMembership[];
  currentLocationId?: string;
  firstName?: string;
};

export type Contact = {
  id: string;
  locationId: string;
  companyId?: string;
  companyName?: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email?: string;
  phone?: string;
  source?: string;
  tags: { id: string; name: string; color?: string }[];
};

export type Company = {
  id: string;
  name: string;
  website?: string;
  phone?: string;
};

export type Tag = {
  id: string;
  name: string;
  color?: string;
};

export type PipelineStage = {
  id: string;
  pipelineId: string;
  name: string;
  sortOrder: number;
};

export type Pipeline = {
  id: string;
  name: string;
  stages: PipelineStage[];
};

export type Opportunity = {
  id: string;
  pipelineId: string;
  pipelineName?: string;
  stageId: string;
  stageName?: string;
  sortOrder: number;
  contactId?: string;
  companyId?: string;
  name: string;
  value: number;
  status: string;
};

export type LocationStats = {
  contactCount: number;
  companyCount: number;
  openOpportunityCount: number;
  openPipelineValue: number;
};
