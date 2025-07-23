export interface LoginDto {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface AuthenticateResultModel {
  accessToken: string;
  encryptedAccessToken: string;
  expireInSeconds: number;
  userId: number;
}