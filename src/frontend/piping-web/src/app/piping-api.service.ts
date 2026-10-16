import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Cml,
  CmlWrite,
  PipingLineDetail,
  PipingLineSummary,
  PipingLineWrite,
  TestPoint,
  TestPointWrite,
  ThicknessReading,
  ThicknessReadingWrite,
} from './models';

@Injectable({ providedIn: 'root' })
export class PipingApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';

  listLines(): Observable<PipingLineSummary[]> {
    return this.http.get<PipingLineSummary[]>(`${this.apiUrl}/piping-lines`);
  }
  getLine(id: number): Observable<PipingLineDetail> {
    return this.http.get<PipingLineDetail>(`${this.apiUrl}/piping-lines/${id}`);
  }
  createLine(value: PipingLineWrite): Observable<{ id: number; lineNumber: string }> {
    return this.http.post<{ id: number; lineNumber: string }>(`${this.apiUrl}/piping-lines`, value);
  }
  updateLine(id: number, value: PipingLineWrite): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/piping-lines/${id}`, value);
  }
  deleteLine(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/piping-lines/${id}`);
  }
  getCml(lineId: number, id: number): Observable<Cml> {
    return this.http.get<Cml>(`${this.apiUrl}/piping-lines/${lineId}/cmls/${id}`);
  }
  createCml(lineId: number, value: CmlWrite): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/piping-lines/${lineId}/cmls`, value);
  }
  updateCml(lineId: number, id: number, value: CmlWrite): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/piping-lines/${lineId}/cmls/${id}`, value);
  }
  deleteCml(lineId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/piping-lines/${lineId}/cmls/${id}`);
  }
  getTestPoint(cmlId: number, id: number): Observable<TestPoint> {
    return this.http.get<TestPoint>(`${this.apiUrl}/cmls/${cmlId}/test-points/${id}`);
  }
  createTestPoint(cmlId: number, value: TestPointWrite): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/cmls/${cmlId}/test-points`, value);
  }
  updateTestPoint(cmlId: number, id: number, value: TestPointWrite): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/cmls/${cmlId}/test-points/${id}`, value);
  }
  deleteTestPoint(cmlId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/cmls/${cmlId}/test-points/${id}`);
  }
  getReading(testPointId: number, id: number): Observable<ThicknessReading> {
    return this.http.get<ThicknessReading>(
      `${this.apiUrl}/test-points/${testPointId}/readings/${id}`,
    );
  }
  createReading(testPointId: number, value: ThicknessReadingWrite): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/test-points/${testPointId}/readings`, value);
  }
  updateReading(testPointId: number, id: number, value: ThicknessReadingWrite): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/test-points/${testPointId}/readings/${id}`, value);
  }
  deleteReading(testPointId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/test-points/${testPointId}/readings/${id}`);
  }
}
