export enum ProjectStatus {
    Active = 1,
    Inactive = 2,
    Completed = 3,
}

export interface Project {
    id: number;
    projectName: string;
    projectDescription?: string;
    projectStatus: ProjectStatus;
    serviceTypeId?: number;
    serviceTypeName?: string;
    createdDate: Date;
    updatedDate: Date;
    companyId: number;
    companyName?: string;
    isActive: boolean;
}

export interface ProjectListItem {
    id: number;
    projectName: string;
    projectDescription?: string;
    projectStatus: ProjectStatus;
    serviceTypeId?: number;
    serviceTypeName?: string;
    createdDate: Date;
    updatedDate: Date;
    isActive: boolean;
    relatedSaleOrdersCount: number;
    companyId: number;
    companyName?: string;
}

export interface CreateProjectDto {
    projectName: string;
    projectDescription?: string;
    projectStatus: ProjectStatus;
    serviceTypeId?: number;
    isActive: boolean;
}

export interface UpdateProjectDto {
    id: number;
    projectName: string;
    projectDescription?: string;
    projectStatus: ProjectStatus;
    serviceTypeId?: number;
    isActive: boolean;
}

export interface CompanyNatureResponse {
    companyNature: string;
    isDigitalMarketing: boolean;
}

export function getProjectStatusText(status: ProjectStatus): string {
    switch (status) {
        case ProjectStatus.Active:
            return 'Active';
        case ProjectStatus.Inactive:
            return 'Inactive';
        case ProjectStatus.Completed:
            return 'Completed';
        default:
            return 'Unknown';
    }
}

export function getProjectStatusClass(status: ProjectStatus): string {
    switch (status) {
        case ProjectStatus.Active:
            return 'success';
        case ProjectStatus.Inactive:
            return 'warning';
        case ProjectStatus.Completed:
            return 'info';
        default:
            return 'secondary';
    }
}
