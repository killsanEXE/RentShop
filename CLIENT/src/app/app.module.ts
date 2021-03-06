import { Injectable, NgModule } from '@angular/core';
import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
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
import { DateInputComponent } from './_forms/date-input/date-input.component';
import {MatPaginatorModule} from '@angular/material/paginator';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminItemComponent } from './admin/admin-item/admin-item.component';
import { AdminUserComponent } from './admin/admin-user/admin-user.component';
import { AdminOrderComponent } from './admin/admin-order/admin-order.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CreateItemComponent } from './admin/items/create-item/create-item.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { UploadItemPhotoComponent } from './admin/items/upload-item-photo/upload-item-photo.component';
import { ItemDetailedComponent } from './orders/item-detailed/item-detailed.component';
import 'hammerjs';
import * as Hammer from "hammerjs";
import { HammerModule } from '@angular/platform-browser';
import { UploadMainPhotoComponent } from './admin/items/upload-main-photo/upload-main-photo.component';
import { GallaryComponent } from './modals/gallary/gallary.component';
import { EditItemComponent } from './admin/items/edit-item/edit-item.component';
import { PointComponent } from './admin/points/point/point.component';
import { EditPointComponent } from './admin/points/edit-point/edit-point.component';
import { AddPointComponent } from './admin/points/add-point/add-point.component';
import { DeletePointComponent } from './admin/points/delete-point/delete-point.component';
import { MatSelectModule } from '@angular/material/select';
import { CreateUnitComponent } from './admin/items/create-unit/create-unit.component';
import { EditUnitComponent } from './admin/items/edit-unit/edit-unit.component';
import { ClientOrdersComponent } from './orders/client-orders/client-orders.component';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatIconModule} from '@angular/material/icon';
import { AddLocationComponent } from './account/add-location/add-location.component';
import { EditLocationComponent } from './account/edit-location/edit-location.component';
import {MatInputModule} from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { BecomeDeliverymanComponent } from './account/become-deliveryman/become-deliveryman.component';
import { OrderComponent } from './orders/order/order.component';
import { DeliverymanMainComponent } from './deliveryman/deliveryman-main/deliveryman-main.component';
import { AvailableOrdersComponent } from './deliveryman/available-orders/available-orders.component';
import { TakeOrdersComponent } from './deliveryman/take-orders/take-orders.component';
import { MessageComponent } from './message/message.component';
import { ReceiveOrderConfirmComponent } from './orders/receive-order-confirm/receive-order-confirm.component';
import { ReturnOrderConfirmComponent } from './orders/return-order-confirm/return-order-confirm.component';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { ExportDatasetComponent } from './admin/export-dataset/export-dataset.component';
import { ForgotPasswordComponent } from './account/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './account/reset-password/reset-password.component';
import { ResendEmailConfirmationComponent } from './account/resend-email-confirmation/resend-email-confirmation.component';
import { MessagesComponent } from './messages/messages.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import {MatButtonModule} from '@angular/material/button';

@Injectable()
export class MyHammerConfig extends HammerGestureConfig  {
  overrides = <any>{
    'swipe': { direction: Hammer.DIRECTION_ALL}
  }
}

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
    UploadMainPhotoComponent,
    GallaryComponent,
    EditItemComponent,
    PointComponent,
    EditPointComponent,
    AddPointComponent,
    DeletePointComponent,
    CreateUnitComponent,
    EditUnitComponent,
    ClientOrdersComponent,
    AddLocationComponent,
    EditLocationComponent,
    BecomeDeliverymanComponent,
    OrderComponent,
    DeliverymanMainComponent,
    AvailableOrdersComponent,
    TakeOrdersComponent,
    MessageComponent,
    ReceiveOrderConfirmComponent,
    ReturnOrderConfirmComponent,
    ExportDatasetComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ResendEmailConfirmationComponent,
    MessagesComponent,
    ServerErrorComponent,
    NotFoundComponent
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
    MatPaginatorModule,
    ModalModule.forRoot(),
    HammerModule,
    MatSelectModule,
    MatProgressBarModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatButtonModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {provide: HAMMER_GESTURE_CONFIG, useClass: MyHammerConfig}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
