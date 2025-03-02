import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { PhotoEditorComponent } from '../members/photo-editor/photo-editor.component';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService =  inject(AccountService);
  baseUrl = environment.apiurl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  getMembers() {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));

    if (response) return this.setPaginatedResponse(response);

    let params = this.setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);

    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy)

    return this.http.get<Member[]>(this.baseUrl + '/users', {observe: 'response', params}).subscribe({
      next: response =>  {
        this.setPaginatedResponse(response);
        this.memberCache.set(Object.values(this.userParams()).join('-'), response);
      }
    });
  }

  resetUserParam() {
    this.userParams.set(new UserParams(this.user));
  }

  private setPaginatedResponse(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!)
    });
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }

  getMember(username: string) {
    // const member = this.members().find((x) => x.userName == username);

    // if (member !== undefined) return of(member);
    const member: Member = [...this.memberCache.values()]
      .reduce((acc, arr) => acc.concat(arr.body), [])
      .find((m: Member) => m.userName === username);
    
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + '/users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + '/users/', member).pipe(
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => (m.userName === member.userName ? member : m))
      //   );
      // })
    );
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseUrl + `/users/set-main-photo/${photo.id}`, {})
      .pipe(
        // tap(() => {
        //   this.members.update((members) =>
        //     members.map((m) => {
        //       if (m.photos.includes(photo)) {
        //         m.photoURL = photo.url;
        //       }
        //       return m;
        //     })
        //   );
        // })
      );
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseUrl + `/users/delete-photo/${photo.id}`, {})
      .pipe(
      //   tap(() => {
      //     this.members.update((members) =>
      //       members.map((m) => {
      //         if (m.photos.includes(photo)) {
      //           m.photos = m.photos.filter(p => p.id != photo.id)
      //         }
      //         return m;
      //       })
      //     );
      //   })
      // );
      )
  }
}
