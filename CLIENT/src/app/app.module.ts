import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModalModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RegisterComponent } from './account/register/register.component';
import { LoginComponent } from './account/login/login.component';
import { HomeComponent } from './home/home.component';
import { RegAndLogComponent } from './account/reg-and-log/reg-and-log.component';
import { MatTabsModule } from '@angular/material/tabs';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProfileComponent } from './account/profile/profile.component';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { HasRoleDirective } from './directives/has-role.directive';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { FileUploadModule } from 'ng2-file-upload';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import {MatPaginatorModule} from '@angular/material/paginator';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminItemComponent } from './admin/admin-item/admin-item.component';
import { AdminUserComponent } from './admin/admin-user/admin-user.component';
import { AdminOrderComponent } from './admin/admin-order/admin-order.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CreateItemComponent } from './admin/create-item/create-item.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { UploadItemPhotoComponent } from './admin/upload-item-photo/upload-item-photo.component';
import { ItemDetailedComponent } from './orders/item-detailed/item-detailed.component';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import 'hammerjs';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    RegAndLogComponent,
    TextInputComponent,
    ProfileComponent,
    HasRoleDirective,
    DateInputComponent,
    AdminPanelComponent,
    AdminItemComponent,
    AdminUserComponent,
    AdminOrderComponent,
    CreateItemComponent,
    ConfirmDialogComponent,
    UploadItemPhotoComponent,
    ItemDetailedComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    NgbModule,
    MatTabsModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    HttpClientModule,
    ToastrModule.forRoot({
      positionClass: "toast-bottom-right"
    }),
    BsDropdownModule.forRoot(),
    FileUploadModule,
    NgxSpinnerModule,
    BsDatepickerModule.forRoot(),
    MatPaginatorModule,
    ModalModule.forRoot(),
    NgxGalleryModule,
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
