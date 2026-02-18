export interface UserPerformanceScore {
    id?: number;
    userId: number;
    userName?: string;
    performanceCriteriaId: number;
    criteriaName?: string;
    score: number;
    achievedValue?: number;
    evaluationPeriod?: string;
    evaluatorUserId?: number;
    evaluatorName?: string;
    comments?: string;
    evaluationDate?: Date;
    createdDate?: Date;
    modifiedDate?: Date;
}

export interface CreateUserPerformanceScoreDto {
    userId: number;
    performanceCriteriaId: number;
    score: number;
    achievedValue?: number;
    evaluationPeriod?: string;
    evaluatorUserId?: number;
    comments?: string;
}

export interface UpdateUserPerformanceScoreDto extends CreateUserPerformanceScoreDto {
    id: number;
}
