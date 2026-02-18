import { UserRole } from './user.model';

export interface PerformanceCriteria {
    id?: number;
    userRole: UserRole;
    name: string;
    description?: string;
    category?: string;
    weightPercentage: number;
    measurementMethod?: string;
    scoringMethod?: string;
    targetValue?: number;
    minimumThreshold?: number;
    maximumThreshold?: number;
    isActive: boolean;
    createdByAI: boolean;
    createdDate?: Date;
    modifiedDate?: Date;
    companyId?: number;
    companyName?: string;
}

export interface CreatePerformanceCriteriaDto {
    userRole: UserRole;
    name: string;
    description?: string;
    category?: string;
    weightPercentage: number;
    measurementMethod?: string;
    scoringMethod?: string;
    targetValue?: number;
    minimumThreshold?: number;
    maximumThreshold?: number;
    createdByAI?: boolean;
}

export interface UpdatePerformanceCriteriaDto extends CreatePerformanceCriteriaDto {
    id: number;
    isActive: boolean;
}
