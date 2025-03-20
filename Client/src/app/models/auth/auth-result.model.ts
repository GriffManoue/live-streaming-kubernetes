export interface AuthResult {
  success: boolean;
  token: string;
  expiresAt: Date;
  userId: string;
  error: string;
}