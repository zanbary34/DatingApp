import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { PaginationHelperService } from './pagination-helper.service';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  private paginationHelper = inject(PaginationHelperService)
  private http = inject(HttpClient);
  baseUrl = environment.apiurl;
  likeIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  likesCache = new Map();

  toggleLike(targetId: Number) {
    return this.http.post(`${this.baseUrl}/likes/${targetId}`, {});
  }
  
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    const response = this.likesCache.get(`${predicate}-${pageNumber}`);

    if (response) {
      this.paginatedResult.set(this.paginationHelper.setPaginatedResponse(response));
      return;
    }

    let params = this.paginationHelper.setPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return this.http.get<Member[]>(`${this.baseUrl}/likes`, {observe: 'response', params}).subscribe({
      next: response =>  {
        this.paginatedResult.set(this.paginationHelper.setPaginatedResponse(response));
        this.likesCache.set(`${predicate}-${pageNumber}`, response);
      }
    });
  }

  getLikesIds() {
    return this.http.get<number[]>(`${this.baseUrl}/likes/list`).subscribe({
      next: ids =>  this.likeIds.set(ids)
    })
  }
}
 