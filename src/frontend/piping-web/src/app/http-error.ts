import { HttpErrorResponse } from '@angular/common/http';

export function toUserMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) return 'เกิดข้อผิดพลาดที่ไม่คาดคิด กรุณาลองอีกครั้ง';
  const payload = error.error as {
    detail?: string;
    message?: string;
    errors?: Record<string, string[]>;
  } | null;
  if (payload?.message) return payload.message;
  if (payload?.detail) return payload.detail;
  if (payload?.errors) return Object.values(payload.errors).flat().join(' ');
  if (error.status === 0) return 'เชื่อมต่อ API ไม่ได้ กรุณาตรวจสอบว่า backend กำลังทำงาน';
  return `คำขอไม่สำเร็จ (HTTP ${error.status})`;
}
