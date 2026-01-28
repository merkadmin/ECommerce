import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map, timeout } from 'rxjs/operators';
import { ConfigService } from './config.service';
import { ApiResponse, PagedResult } from '../models';
import { ApiController, ApiAction, ErrorHandleLocation } from '../enums';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly defaultTimeout = 30000; // 30 seconds

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  private get baseUrl(): string {
    return this.configService.apiBaseUrl;
  }

  private buildUrl(controller: ApiController | string, action?: string): string {
    let url = `${this.baseUrl}/${controller}`;
    if (action) {
      url += `/${action}`;
    }
    return url;
  }

  private handleError(error: any, errorHandler: ErrorHandleLocation): Observable<never> {
    if (errorHandler === ErrorHandleLocation.Caller) {
      return throwError(() => error);
    }
    // Interceptor will handle the error
    return throwError(() => error);
  }

  // GET all records
  getAll<T>(
    controller: ApiController | string,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T[]> {
    return this.http.get<ApiResponse<T[]>>(this.buildUrl(controller)).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // GET with pagination
  getAllPaged<T>(
    controller: ApiController | string,
    page: number = 1,
    pageSize: number = 20,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<PagedResult<T>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<PagedResult<T>>>(this.buildUrl(controller), { params }).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // GET by ID
  getById<T>(
    controller: ApiController | string,
    id: string,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.buildUrl(controller)}/${id}`).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // GET with params
  getByParams<T>(
    controller: ApiController | string,
    action: string,
    params: HttpParams | { [key: string]: string | string[] },
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T> {
    const httpParams = params instanceof HttpParams ? params : new HttpParams({ fromObject: params });

    return this.http.get<ApiResponse<T>>(this.buildUrl(controller, action), { params: httpParams }).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // POST - Insert
  insert<T>(
    controller: ApiController | string,
    data: any,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.buildUrl(controller), data).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // PUT - Update
  update<T>(
    controller: ApiController | string,
    id: string,
    data: any,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.buildUrl(controller)}/${id}`, data).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // DELETE
  delete(
    controller: ApiController | string,
    id: string,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<boolean> {
    return this.http.delete<ApiResponse<boolean>>(`${this.buildUrl(controller)}/${id}`).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // POST with action
  postWithAction<T>(
    controller: ApiController | string,
    action: string,
    data: any,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.buildUrl(controller, action), data).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }

  // Search with filters
  search<T>(
    controller: ApiController | string,
    filters: any,
    errorHandler: ErrorHandleLocation = ErrorHandleLocation.Interceptor
  ): Observable<PagedResult<T>> {
    return this.http.post<ApiResponse<PagedResult<T>>>(
      this.buildUrl(controller, 'search'),
      filters
    ).pipe(
      timeout(this.defaultTimeout),
      map(response => response.data),
      catchError(error => this.handleError(error, errorHandler))
    );
  }
}
