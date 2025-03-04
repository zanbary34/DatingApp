import { Injectable } from '@angular/core';
import { Member } from '../_models/member';
import { HttpParams, HttpResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class PaginationHelperService {
  setPaginatedResponse<T>(response: HttpResponse<T[]>) {
    return {
      items: response.body as T[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    };
  }

  setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }
}
