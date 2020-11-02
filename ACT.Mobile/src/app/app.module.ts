import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';

import { File } from '@ionic-native/file/ngx';
import { Camera } from '@ionic-native/camera/ngx';
import { IonicStorageModule } from '@ionic/storage';
import { HttpClientModule } from '@angular/common/http';
import { AppVersion } from '@ionic-native/app-version/ngx';
import { FileOpener } from '@ionic-native/file-opener/ngx';
import { FingerprintAIO } from '@ionic-native/fingerprint-aio/ngx';
import { FileTransfer, FileTransferObject } from '@ionic-native/file-transfer/ngx';

import { UserService } from '../app/services/user.service';

import { WelcomeComponent } from './welcome/welcome.component';
import { CreatepinPage } from './createpin/createpin.page';
import { ResetpinconfirmComponent } from './resetpinconfirm/resetpinconfirm.component';
import { ExitappconfirmComponent } from './exitappconfirm/exitappconfirm.component';

import { LaunchNavigator } from '@ionic-native/launch-navigator/ngx';
import { PhotoViewer } from '@ionic-native/photo-viewer/ngx';
import { PhotoLibrary } from '@ionic-native/photo-library/ngx';
import { CallNumber } from '@ionic-native/call-number/ngx';
import { ContactactComponent } from './contactact/contactact.component';
import { ConfirmdeletesiteauditComponent } from './confirmdeletesiteaudit/confirmdeletesiteaudit.component';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    ResetpinconfirmComponent,
    CreatepinPage,
    ExitappconfirmComponent,
    ContactactComponent,
    ConfirmdeletesiteauditComponent
  ],
  entryComponents: [
    WelcomeComponent,
    ResetpinconfirmComponent,
    CreatepinPage,
    ExitappconfirmComponent,
    ContactactComponent,
    ConfirmdeletesiteauditComponent
  ],
  imports: [
    BrowserModule,
    IonicModule.forRoot(),
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    IonicStorageModule.forRoot()
  ],
  providers: [
    StatusBar,
    SplashScreen,
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    UserService,
    Camera,
    FileTransfer,
    FileTransferObject,
    File,
    AppVersion,
    FileOpener,
    FingerprintAIO,
    LaunchNavigator,
    PhotoViewer,
    PhotoLibrary,
    CallNumber
  ],
  bootstrap: [AppComponent]
})
export class AppModule
{
  
}
